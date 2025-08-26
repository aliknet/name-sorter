# Name Sorter

A tiny, SOLID .NET console app that sorts names by Last name, then by Given names.
Each name must have a Last Name and at least 1 given name.
Program supports up to 3 given names.
A sample file named unsorted-names-list.txt is provided.

The solution includes 2 Projects:
- name-sorter : The Main sorter application
- name-sorter.tests : The Tests Project using xUnit and Moq

# Input
A file listing all names, one name per line

# Output
A sorted file named "sorted-names-list.txt" listing all names sorted, one name per line

## Requirements
- .NET 8 SDK

## Build & Test
dotnet build
dotnet test

## Usage
dotnet run --project name-sorter ./unsorted-names-list.txt