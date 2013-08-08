File-Information-App
====================

This C# console application recursively searches a user-defined directory and all of its subdirectories for 
Jpeg and PDF files by reading the byte stream of every file in those directories. It gets the attributes
(full file name, MD5 hash, etc) of each of those files, creates a next text file, and adds the attributes 
to the text file as comma-separated values.

First input: Enter the path of the directory to be searched. The application will recursively search all
subdirectories in the main directory.

Second input: Enter the path where you want the text file created. 
