using System; // Imports the fundamental System namespace containing the base Exception class

namespace MondayCsvReader // Keeps code organized within your specific project group
{ 
    internal class InvalidUsageRecordException : Exception // Creates a custom exception class that inherits from the built-in Exception class
    { 
        public int LineNumber { get; } // Defines a public property to store the exact line number where the error occurred

        // Defines a constructor requiring an error message and the file line number
        public InvalidUsageRecordException(string message, int lineNumber)
            : base($"{message} (Line {lineNumber})") // Passes the custom combined message string directly up to the base Exception class
        { 
            LineNumber = lineNumber; // Saves the passed-in line number variable into the class property for later reporting
        } 

        // Defines an advanced constructor that accepts an inner exception to preserve the original system error history
        public InvalidUsageRecordException(string message, int lineNumber, Exception innerException)
            : base($"{message} (Line {lineNumber})", innerException) // Passes both the message and the inner system error up to the base class
        { 
            LineNumber = lineNumber; // Saves the line number into the class property so it isn't lost during error propagation
        } 
    } 
} 
