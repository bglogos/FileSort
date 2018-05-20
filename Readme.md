# Console text file sorter

.NET Core console application that sorts given text file.

# Usage

The application sorts alphabetically the lines in text files of any size. E.g. file containing

```
lorem
ipsum
dolor
foo
bar
```

will be sorted like

```
bar
dolor
foo
ipsum
lorem
```


Because only small parts of the file are read in memory, free disk space at least double the size of the input file is required to successfully complete the operation.

# Notes

Due to the very limited time I had, because of other personal issues, this solution is far from perfect and the sorting algorithm could be broken easily. It also lacks quality ensuring like unit or integration testing. Still, I decided to submit it for review of the basic idea.