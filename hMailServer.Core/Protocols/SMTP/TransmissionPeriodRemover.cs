using System;
using System.IO;
using System.Text;

namespace hMailServer.Core.Protocols.SMTP
{
    /// <summary>
    /// In some mail protocols (POP3 and IMAP), dots are escaped in the transmission of the data.
    /// If a line starts with two dots, then one of them should be removed. This is because a dot
    /// on an empty line otherwise indicates end-of-transmission.
    /// </summary>
    public static class TransmissionPeriodRemover
    {
        public static void Process(Stream input, Stream output, int inputBytesToProcess)
        {
            if (inputBytesToProcess > input.Length)
                throw new ArgumentException("inputBytesToProcess > input.Length", nameof(inputBytesToProcess));
            
            int characterFilter = '\n';

            for (int position = 0; position < inputBytesToProcess; position++)
            {
                int character = input.ReadByte();

                if (character == -1)
                    throw new Exception("Reached end of buffer before flush position was reached.");

                if (character == '\n')
                {
                    characterFilter = '\n';
                }
                else if (characterFilter == '\n' && character == '.')
                {
                    characterFilter = '.';
                }
                else if (characterFilter == '.' && character == '.')
                {
                    // We've already reached a dot on a new line. 
                    // If two dots are found on a new line, skip the second
                    continue;
                }
                else
                {
                    characterFilter = '\0';
                }

                output.WriteByte(Convert.ToByte(character));
    
            }

        }
    }
}
