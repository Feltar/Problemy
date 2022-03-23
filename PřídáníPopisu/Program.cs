using PřídáníPopisu;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
//Description can be found in the respective solutions.






//pentagon [0,1,2,3,4] abscisa [3,9], pentagon [5,6,7,8,11]
bool[,] matrixFinal = new bool[12, 12];
matrixFinal[0, 1] = true; matrixFinal[0, 4] = true;
matrixFinal[1, 2] = true; matrixFinal[1, 0] = true;
matrixFinal[2, 3] = true; matrixFinal[2, 4] = true; matrixFinal[2, 1] = true;
matrixFinal[3, 9] = true; matrixFinal[3, 2] = true; matrixFinal[3, 4] = true;
matrixFinal[4, 0] = true; matrixFinal[4, 3] = true; matrixFinal[4, 2] = true;
matrixFinal[9, 3] = true;


matrixFinal[5, 6] = true; matrixFinal[5, 11] = true;
matrixFinal[6, 7] = true; matrixFinal[6, 5] = true;
matrixFinal[7, 6] = true; matrixFinal[7, 8] = true;
matrixFinal[8, 11] = true; matrixFinal[8, 7] = true;
matrixFinal[11, 8] = true; matrixFinal[11, 5] = true;


Console.WriteLine("The maximum size of graph component is " + GraphSolution.findTheMaximumSizeOfComponent(matrixFinal));






//point [0]
bool[,] matrix4 = new bool[,] { { false } };
Console.WriteLine("The maximum size of graph component is " + GraphSolution.findTheMaximumSizeOfComponent(matrix4));

//abscisa [0,1]
bool[,] matrix3 = new bool[,] { { false, true }, { true, false } };
Console.WriteLine("The maximum size of graph component is " + GraphSolution.findTheMaximumSizeOfComponent(matrix3));

//abscissa [0,1] and triangle [2,3,4]
bool[,] matrix = new bool[, ] { {false, true, false, false, false }, 
                                {true, false, false, false, false }, 
                                {false, false,false, true, true },
                                {false, false, true,false, false  },
                                {false,false, true, false, false } 
                            };

Console.WriteLine("The maximum size of graph component is "+ GraphSolution.findTheMaximumSizeOfComponent(matrix));



//triangle [0,1,2] and a line abscissa [2, 3]
bool[,] matrix2 = new bool[,] {
                        { false, true, true, false},
                        {true, false, false, false },
                        { true, false,false, true},
                        {false, false, true, false }
                    };
Console.WriteLine("The maximum size of graph component is " + GraphSolution.findTheMaximumSizeOfComponent(matrix2));









string connectionString = ConfigurationManager.ConnectionStrings["RetezecSpojeniPokusnaDatabaze"].ConnectionString;
SqlConnection con = new SqlConnection(connectionString);

string SQLQuery = "SELECT Id FROM TestDataTable";

List<int> lisOfIds = con.Query<int>(SQLQuery).ToList();
List<string> listOfDescriptions = lisOfIds.Select(x => "This is a description of the elemnt with the id " + x).ToList();

SoultionSQL.addsCommentColumn(lisOfIds, listOfDescriptions);
