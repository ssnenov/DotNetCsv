using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace DotNetCsv
{
    public class CsvReader<T> : ICsvReader<T>, IDisposable where T : new()
    {
        private static PropertyInfo iListIndexerCache = typeof(IList<object>).GetProperties().First(x => x.GetIndexParameters().Length > 0);

        private readonly BasicCsvReader csvReader = new BasicCsvReader();

        private PropertyInfo[] columnToPropertiesCache;

        private int knownColumnsCount;

        private Func<IList<object>, T> objectCreationCache;

        private List<object> propertyValuesCache;

        private TypeConverter[] propertiesConvertersCache;

        public IEnumerable<T> Read(TextReader textReader)
        {
            using (var rowsEnumerator = this.csvReader.Read(textReader).GetEnumerator())
            {
                if (!rowsEnumerator.MoveNext())
                { // TODO: add support for CSVs without header row
                    throw new InvalidOperationException("Could not read first row");
                }

                var columnNames = rowsEnumerator.Current;
                var columnsCount = columnNames.Count;
                var columnToProperties = columnToPropertiesCache ?? (columnToPropertiesCache = GetColumnToProperties(columnNames));
                var knownPropertiesCount = this.knownColumnsCount == 0 ? (this.knownColumnsCount = columnToProperties.Count(x => x != null)) : this.knownColumnsCount;
                var propertyValues = this.propertyValuesCache ?? (this.propertyValuesCache = new List<object>(knownPropertiesCount));
                var propertiesConverters = this.propertiesConvertersCache ?? (propertiesConvertersCache = this.GetPropertiesConverters(columnToProperties));
                Func<IList<object>, T> objectCreation = objectCreationCache ?? (objectCreationCache = GetConstructionMethod(columnToProperties));

                while (rowsEnumerator.MoveNext())
                {
                    int rowCellsCount = rowsEnumerator.Current.Count;
                    for (int i = 0; i < rowCellsCount; i++)
                    {
                        if (propertyValues.Count <= knownPropertiesCount && i < columnsCount && columnToProperties[i] != null)
                        // Skip row values outside of a table -> not correspond to column and those without properties in the model
                        {
                            string propertyValueString = rowsEnumerator.Current[i];
                            TypeConverter propertyConverter = propertiesConverters[i];
                            if (propertyConverter != null)
                            {
                                try
                                {
                                    propertyValues.Add(propertyConverter.ConvertFromString(propertyValueString));
                                }
                                catch (Exception)
                                {
                                    propertyValues.Add(null);
                                    // TODO: log the exception
                                }
                            }
                            else
                            {
                                propertyValues.Add(propertyValueString);
                            }
                        }
                    }

                    T result;
                    try
                    {
                        result = objectCreation(propertyValues);
                    }
                    catch (Exception)
                    {
                        continue;
                        // TODO: log the exception
                    }

                    yield return result;

                    propertyValues.Clear();
                }
            }
        }

        private TypeConverter[] GetPropertiesConverters(PropertyInfo[] columnToProperties)
        {
            var propertiesDescriptors = new TypeConverter[columnToProperties.Length];
            for (int i = 0; i < columnToProperties.Length; i++)
            {
                if (columnToProperties[i] == null)
                {
                    continue;
                }

                Type propertyType = columnToProperties[i].PropertyType;
                if (propertyType != typeof(string))
                {
                    propertiesDescriptors[i] = TypeDescriptor.GetConverter(propertyType);
                }
            }

            return propertiesDescriptors;
        }

        private static PropertyInfo[] GetColumnToProperties(IList<string> columnNames)
        {
            var dataMemberProperties = new PropertyInfo[columnNames.Count];
            foreach (var property in typeof(T).GetProperties())
            {
                var dataMember = new Lazy<DataMemberAttribute>(() => (DataMemberAttribute)Attribute.GetCustomAttribute(property, typeof(DataMemberAttribute), false)); // TODO: Make inherit param to be a configuration;
                for (int i = 0; i < columnNames.Count; i++)
                {
                    if (property.Name == columnNames[i])
                    {
                        dataMemberProperties[i] = property;
                        break;
                    }
                    else if (dataMember.Value != null && (dataMember.Value.Name == columnNames[i] || dataMember.Value.Order == i))
                    {
                        dataMemberProperties[i] = property;
                        break;
                    }
                }
            }

            return dataMemberProperties;
        }

        public static Func<IList<object>, T> GetConstructionMethod(PropertyInfo[] properties)
        {
            var propertyValues = Expression.Parameter(typeof(IList<object>), "propertyValues");
            var modelCreationExpression = Expression.New(typeof(T));

            var assignmentExpression = new List<MemberBinding>(properties.Length);
            int skipped = 0;
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i] == null)
                {
                    skipped++;
                    continue;
                }

                var propertyValueExpression = Expression.MakeIndex(propertyValues, iListIndexerCache, new[] { Expression.Constant(i - skipped) });
                assignmentExpression.Add(Expression.Bind(properties[i], Expression.Convert(propertyValueExpression, properties[i].PropertyType)));
            }

            var memberInit = Expression.MemberInit(modelCreationExpression, assignmentExpression);

            return (Func<IList<object>, T>)Expression.Lambda(memberInit, propertyValues).Compile();
        }

        public virtual void Dispose() => this.csvReader?.Dispose();
    }
}