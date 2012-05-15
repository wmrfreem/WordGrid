WordGrid
========

C# Project/Library for generating and displaying all m-by-n word grids given a file containing a list of words.
Ex. 

	 dohs
	 oboe
	 gone
	 seek

Algorithm Overview:

	- Filter the list of words into two List<T> objects whose elements are all of length m, n respectively
	- Store each word underlyingly as an int[] representing lowercase alphabetic characters, where: a->0, b-> 1,..., z->25
	
	- Pick the larger of m and n, set this as the primary dimension, call this size p, and the smaller size s
	- Initialize a flag primary to true that indicates if we are working on the primary dimension
	- Iterate along the depth d from 0 to s
		- pick the working word list as indicated by the flag primary
		- pick the first word from the word list which fits in the grid at depth d. In the case of an empty grid,
		the very first word is chosen. If there is no such word, decrement d
		- if d == s, then the grid is full along one dimension => it is full, but we are not sure if all the words in the
		other dimension are in fact words. Check these words and print the current grid to stdout if they exist. Decrement d
		- alternate the flag primary
	- Terminate the loop once the current word in the primary dimension is the last word in the list
	
	- logic implemented in Grid.Generate()
	
Usage:

	- wordgrid.exe m n
	- Ex. wordgrid.exe 3 3
	- The file dict.txt is expected to be in the same folder as wordgrid.exe
		- Feel free to supply your own word list and compare the number of grids
	- Sample output for 3x3, 3x4, 4x4 grids are provided in sampleoutput/.
		
References:

	- Word list file taken from http://www.mieliestronk.com/corncob_lowercase.zip	
		