# QueryMyData
![Query Image](https://imgur.com/4j7jiTs.jpg)

## What is this?

An application which can read data from excel files, incorporate them into a DataSet and create an SQL table from said DataSet. The user can retrieve data via queries to .XML or .CSV files. 

## Tech Used
- C#
- WPF
- DataSets
- SQLClient

## Features

#### Read file:<br>
Reads the file and converts it to a DataSet, if not already created, it creates a unique Database and Table.
#### Query to CSV or XML:<br>
Retrieves the data from said database / table and converts it to a .csv or .xml file. The output folder is currently the Desktop.
#### Data to CSV or XML:<br>
Converts the Dataset read from a file to a .csv or .xml file. Outputs to the Desktop.
#### Query:<br>
Here you can enter the query, by pressing enter applying it for further usage. An "Apply" button has also been added for this purpose, but pressing enter just works fine as well.
#### Guie:<br>
Usage guide for the query input.
#### Table Info:<br>
The currently available Tables and their Columns are listed here. Changes upon reading a file.

-Constraints:<br>
The user is unable to use CREATE, ALTER or DROP queries. The query's last character must be a ";".

## Installation

To Install:
- Clone the most updated branch.
- Copy or clone to a folder of your choice.
- Read the .sln file in Visual Studio
- Compile the application to a .exe file or run it via the IDE

## Further plans:

- Refactoring the code
- Implementing dropping tables and databases
- Reworking query handling
