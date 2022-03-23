using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PřídáníPopisu
{
    /* Let G = (V,E) be an unoriented graph, where V = {1,2,…,n} and let us consider a matrix of data type boolean and measures nxn, so that
	            for each i, j \in {0,1,2,…, n}: M(i, j) = true, 
            if and only if (i, j) \in V. 
        Find the size of a maximal component of Graph G.
    
        Observation: Matrix M is symmetrical.
    
        Solution:
        The algorithm is implemented as a breadth search.
        We keep a Colors array, that helps us remember which  Vertices we have already visited 
            and SizeOfComponent array, that keeps the size of particular components 
        We go through the vertices and apply edges that go to vertices whose nominal value is smaller than the vertex, we examine at that moment.

        Algorithm:
        Let us define a color of each vertex as a number that is eaqual to the nominal value of that vertex.
    
        We start at the index col = 0 and increment to n-1
            For each of the col values we color all the indexes that vertex col is connected with and whose nominal value is greater than col. 
                For each such action (recolouring of a vertex) we increment the SizeOfComponent corresponding to the col value by one. 
    
        At the end of this we look which color corresponds to the largest size of component.
            

        I would argure that the exact time complexity depends on the characterics of the input. 
        In our case we are going to use the matrix of coincidence. For each element of the graph we are going to make at most n operations, so
        its time complexity is O(n^2). If the input would take form of an array of edges, I believe the time complexity would be O(m n), with m being the length of the array of edges. 
     */



    class GraphSolution
    {

        static public int findTheMaximumSizeOfComponent(bool[,] matrixOfCoincidence)
        {
            //input validation
            int numRows = matrixOfCoincidence.GetLength(0), numCols = matrixOfCoincidence.GetLength(1);
            if (numRows != numCols) { throw new Exception("You must pass a square matrix as the parameter."); }


            int[] Colors = Enumerable.Range(0, numRows).ToArray(); //returns an array with values 0,1,2,...,numRows-1
            int[] SizesOfComponents = Enumerable.Repeat(1, numRows).ToArray(); //returns an array with values 1,1,1,1....,1 (times numRows)

            for (int col = 0; col < numRows-1; col++)
            {
                for (int row = col+1; row < numRows; row++)
                {
                    if (matrixOfCoincidence[row, col] && Colors[row]!=Colors[col]) { 
                            Colors[row] = Colors[col]; 
                            SizesOfComponents[Colors[col]]++; 
                    }
                }
            }

            return SizesOfComponents.Max();
        }
    }
}
