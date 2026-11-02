using System; // Imports base system utilities like Console text output
using System.IO; // Imports file handling tools like StreamReader and File manipulation classes

namespace MondayCsvReader // Matches the namespace of your exception class so they can see each other
{ 
    internal class Program // Defines the primary class that holds the execution logic of the application
    { 
        static void Main() 
        { 
            string filePath = "usage_records.csv"; // Stores the target file name inside a text variable

            CreateSampleCsv(filePath); // Calls our helper method at the bottom to generate a fake test file automatically

            int totalLines = 0; // Initializes a counter integer to track every single row read from the file
            int badLines = 0; // Initializes a counter integer to track how many rows failed validation checks

            Console.WriteLine(" Starting CSV Processing "); 

            using (StreamReader reader = new StreamReader(filePath)) // Opens the file safely and guarantees it closes even if a crash happens
            { 
                string line; // Allocates a text variable to hold the contents of the current line temporarily

                while ((line = reader.ReadLine()) != null) // Loops repeatedly, reading the next file row until it hits the end of the file
                { 
                    totalLines++; // Increments the total line counter by 1 for each row encountered

                    try // Starts a protected execution zone where errors will be intercepted rather than crashing the app
                    { 
                        ProcessLine(line, totalLines); // Passes the line text and its sequence number to the validation manager
                    } 
                    catch (InvalidUsageRecordException ex) // Catches our custom exception if validation failed on this specific line
                    { 
                        badLines++; // Increments our counter to record that another corrupt record was successfully skipped
                        Console.WriteLine($"[SKIPPED] Error: {ex.Message}"); // Prints out the customized error message detailing what was wrong
                    } 
                } 
            } 

            Console.WriteLine("\nFinal Report"); // Prints a summary section divider line to the console screen
            Console.WriteLine($"Total Rows Checked: {totalLines}"); // Displays the total count of lines processed by the loop
            Console.WriteLine($"Successfully Read: {totalLines - badLines}"); // Displays the math showing total lines minus the bad ones
            Console.WriteLine($"Skipped Bad Rows: {badLines}"); // Displays the final tally of malformed data lines that were bypassed
        } // Closes the Main method block

        static void ProcessLine(string line, int lineNumber) // Defines the validation method that inspects text rows and throws custom exceptions
        { 
            if (string.IsNullOrWhiteSpace(line)) // Evaluates if the row is completely blank or filled purely with blank spacing characters
            { 
                throw new InvalidUsageRecordException("Line is entirely empty.", lineNumber); // Halts execution here and alerts the system using our custom exception
            } 

            string[] fields = line.Split(','); // Cuts the string line into an array of smaller pieces wherever a comma character is found

            if (fields.Length != 3) // Checks if the split array has any count other than exactly 3 required columns
            { 
                throw new InvalidUsageRecordException($"Invalid columns. Expected 3, found {fields.Length}.", lineNumber); // Throws the custom error showing the structural breakdown
            } 
            if (!int.TryParse(fields[0], out _)) // Attempts to parse the first column into a number; if it fails, enters the block
            { 
                throw new InvalidUsageRecordException($"Record ID '{fields[0]}' must be an integer numeric value.", lineNumber); // Throws the custom exception pinpointing the invalid format
            } 
        } 

        static void CreateSampleCsv(string path) // Defines a quick utility method to create sample data for immediate testing
        { 
            string[] testRows = { // Initializes a string array holding an assortment of clean data and bad rows
                "101,45.5,12.50", // Row 1: Perfectly valid data pattern
                "BAD_ID,12.0,5.00", // Row 2: Invalid text inside the ID slot (Should trigger format exception)
                "103,90.2", // Row 3: Missing a column entirely (Should trigger column count exception)
                "", // Row 4: Completely blank line (Should trigger empty row exception)
                "104,15.0,2.30" // Row 5: Perfectly valid data pattern
            }; // Ends the string array literal specification
            File.WriteAllLines(path, testRows); // Overwrites or generates the physical file on your disk using the array rows
        } 
    } 
} 
