using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace DotNetCsv
{
    public unsafe class BasicCsvReader : ICsvReader<IList<string>>
    {
        private const int ReaderBlockBufferSize = 128;
        private bool isEnclosedQuotesValue;

        // used for flag indicating is it a first or second quoted from escaping quote - "". It's false when odd number of quotes are found within a value and true when even number.
        // In other words:
        //      first quote in the sequence => false
        //      second quote in the sequence => true
        private bool isEvenQuote;

        private List<string> rowCells = new List<string>(); // TODO: add cells count as configuration
        private char[] cellValueBuffer = new char[256]; // 256*2byte (1char = 2bytes) = 512bytes TODO: add buffer size as configuration
        private char[] readerBuffer = new char[ReaderBlockBufferSize];
        private char* cellValueBufferPtr;
        private char* cellValueBufferStartPtr;
        private int currentCharIndex = 0;
        private GCHandle gcHandle;
        private bool immutableRows = false;

        public BasicCsvReader()
        {
            fixed (char* ptr = &this.cellValueBuffer[0])
            {
                gcHandle = GCHandle.Alloc(cellValueBuffer, GCHandleType.Pinned);
                this.cellValueBufferStartPtr = this.cellValueBufferPtr = ptr;
            }
        }

        public IEnumerable<IList<string>> Read(TextReader textReader)
        {
            int bufferLength;
            int offset = 0;
            int lastlyReadBufferLength = 0;
            while ((bufferLength = textReader.ReadBlock(readerBuffer, offset, ReaderBlockBufferSize - offset)) > 0)
            {
                lastlyReadBufferLength = bufferLength;
                for (int i = 0; i < bufferLength; i++)
                {
                    char currentChar = readerBuffer[i];

                    if (currentChar == '"')
                    {
                        ReadEnclosedQuoteValue(currentChar, i);
                    }
                    else if (currentChar == ',' && !isEnclosedQuotesValue) // TODO: add support for separator character - , or ;
                    {
                        ReadCellValue();
                    }
                    else if (!isEnclosedQuotesValue && currentChar == '\r' && readerBuffer[i + 1] == '\n') // Reading new line. TODO: Add line ending as an option (\r\n or\n)
                    {
                        i++; // Skipping \n character

                        if (currentCharIndex != 0)
                        {
                            ReadCellValue();
                        }

                        if(this.immutableRows) {
                            yield return this.rowCells;
                            this.rowCells = new List<string>(this.rowCells.Count); // creating new object, since the lastly used is returned and could be changed/enumerated by the user later
                        } else {
                            yield return this.rowCells;
                            this.rowCells.Clear();
                        }
                    }
                    else
                    {
                        ReadChar(currentChar);
                    }
                }

                offset = 1; // Reserve one char for peeking
                readerBuffer[0] = readerBuffer[ReaderBlockBufferSize - 1]; // Move the last char (peeking char) from the last to the first position
            }

            if (currentCharIndex != 0)
            {
                if (lastlyReadBufferLength < ReaderBlockBufferSize) // It will be false only if the CSV length is exactly the ReaderBlockBufferSize
                {
                    ReadChar(readerBuffer[lastlyReadBufferLength]); // Reading the last peeking char
                }

                ReadCellValue();
            }

            if (rowCells.Count != 0)
            {
                // TODO: this is a bug. It should use immutableRows to determine the correct behavior
                yield return rowCells;
                rowCells = new List<string>(rowCells.Count);
            }

            // TODO: free it on Dispose
            //gcHandle.Free();
        }

        private void ReadChar(char currentChar)
        {
            if (currentCharIndex >= this.cellValueBuffer.Length - 1) // Ensure enough capacity
            {
                gcHandle.Free(); // Free it as soon as possible. Give the GC option to move the buffer in case of GC occurred during initialization of the new resized buffer
                var resizedBuffer = new char[this.cellValueBuffer.Length * 2];
                this.cellValueBuffer.CopyTo(resizedBuffer, 0);
                this.cellValueBuffer = resizedBuffer;

                fixed (char* ptr = &this.cellValueBuffer[0])
                {
                    this.cellValueBufferStartPtr = this.cellValueBufferPtr = ptr;
                    gcHandle = GCHandle.Alloc(this.cellValueBuffer, GCHandleType.Pinned);
                }
            }

            *(this.cellValueBufferPtr++) = currentChar;
            this.currentCharIndex++;
        }

        private void ReadEnclosedQuoteValue(char currentChar, int readerBufferPos)
        {
            if (this.isEnclosedQuotesValue)
            {
                // If double quotes are enclosed in a value it should be escaped with preceding it with another double quote: "aa""aa"
                if (!this.isEvenQuote && readerBuffer[readerBufferPos + 1] == '"') // If next char is a double quoute, then read the current (preceding one)
                {
                    this.isEvenQuote = !this.isEvenQuote;
                    ReadChar(currentChar);
                }
                else if (currentCharIndex > 0 && this.isEvenQuote && *(this.cellValueBufferPtr - 1) == '"') // if the previous char was quote (preceding one for escaping)
                {
                    this.isEvenQuote = !this.isEvenQuote;
                    // Second quote and we will skip it
                }
                else
                {
                    // Skipping read the closing quote character
                    this.isEnclosedQuotesValue = false;
                    this.isEvenQuote = false; // setting it just in case, to not break next entries
                }
            }
            else
            {
                this.isEnclosedQuotesValue = true;
            }
        }

        private void ReadCellValue()
        {
            rowCells.Add(new string(this.cellValueBufferStartPtr, 0, currentCharIndex));

            this.cellValueBufferPtr = this.cellValueBufferStartPtr;
            currentCharIndex = 0;
        }
    }
}