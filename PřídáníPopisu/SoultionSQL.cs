using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PřídáníPopisu { 
    //Při pohovoru jsi se mě dotazoval, jak bych přidal programačně do nějaký databáze za běhu sloupec Poznámka, kdybych chtěl, aby nemohl obsahovat null hodnoty a
    //kdybych do toho sloupce posléze chtěl přidat nějaké hodnoty za běhu.
    //Pak jsi to doplnil komentářem, že ty bys to dělal v rámci transakce. Popravdě jsem si to chtěl zkusit, tak jsem si k tomu dneska sednul
    //a narazil jsem na jeden problém. Moje interpretace:
    
    //Jestli to chápu správně, tak když chci spustit více dotazů v rámci jedný transakce, tak jsou ty dotazy parsovaný v jedný dávce (batch).
    //Ten server prostě ty metadata ohledně příslušný tabulky načte jenom jednou (na začátku tý transakce).
    //To znamená, že když pustím DDL dotaz na přidání sloupce, tak i přesto, že vše proběhne v pořádku, tak
    //mi ten Server při odvální se na ten sloupec při následném updatu vyhodí chybu - právě kvůli tomu, že tam ten sloupec nebyl na začátku transakce.
    //Dočetl jsem se, že to jde nějak řešit použitím GO delimiteru, který odděluje jednotlivý dávky.
    //Tam je ovšem probĺém, že ADO.NET, ani Dapper (což je jenom extension knihovna ADO) to nějak neuměj zpracovat. Jinými slovy to ignorujou.
    //Našel jsem, že SMO server s tim nějak umí zacházet, ale tomu zas údajně nejdou předávat parametry. Takže jsem tam musel ty stringy označující popis
    //dát na pevno. Což se mi nezdá moc elegantní (Ač to funguje).
    //Je nějaký postup, který by bylo v C# lepší použít místo toho? Případně bych tě požádal jestli bys mě odkázal ohledně toho na nějakou literaturu/blog post/video/...?
    //Předem děkuji.


    //Poznámka: Normálně bych ty skripty alespoň uložil do vlastních souborů, jedná se však o toy example. Nebral jsem to tedy jako příliš důležité.





    static class SoultionSQL
    {
        /// <summary>
        /// This method first finds out wether there already is defined a column named Comment in the TestDataTable of the TestDatabase.
        /// If there is, then it terminates. If not, then it alters the data table, so it contains a column named Comment with a NOT NULL constraint.Then it assigns to it the  
        /// descriptions given as parameters. By default a record not contained in the listOfIds is going to contain a string 'default value'. 
        /// </summary>
        /// <param name="listOfIds">Ids of records, that we want to assign descriptions to.</param>
        /// <param name="listOfDescriptions">Descriptions that are going to be stored in the Comment column. 
        ///                                Let us take a record x in the TestDataTable.
        ///                                It is required, that the index of Id of x (in the listOfIds) ïs the same as the index 
        ///                                of description of x  (in the listOfDescriptions).
        /// </param>
        static public void addsCommentColumn(List<int> listOfIds, List<string> listOfDescriptions)
        {
            if (listOfIds.Count != listOfDescriptions.Count) throw new Exception("Input error: The listOfIds and listOfDescriptions must have the saame length.");
            if (listOfDescriptions.Contains(null)) throw new Exception("Input error: the listOfDescriptions can not contain a null value.");

            string retezecSpojeni = ConfigurationManager.ConnectionStrings["RetezecSpojeniPokusnaDatabaze"].ConnectionString;
            using (Microsoft.Data.SqlClient.SqlConnection connection = new Microsoft.Data.SqlClient.SqlConnection(retezecSpojeni))
            {
                Microsoft.SqlServer.Management.Smo.Server server = new Server(new ServerConnection(connection));
                try
                {
                    //first we need to find out wether the column we are trying to add to the table, does not already exist (in the schema of the table)
                    string sqlDoesSchemaContainColumnNamedComment =
                                       "IF EXISTS( " +
                                            "SELECT " +
                                                "* FROM INFORMATION_SCHEMA.COLUMNS " +
                                           "WHERE " +
                                                "table_name = 'TestDataTable' AND column_name = 'Comment'" +
                                        ")" +
                                           "select " +
                                                "cast((1) as bit) " +
                                           "ELSE " +
                                                "select cast((0) as bit)";
                    if ((bool)server.ConnectionContext.ExecuteScalar(sqlDoesSchemaContainColumnNamedComment))
                    {
                        Console.WriteLine("There is already a column named Comment inside of the DataTable.");
                    }
                    else
                    {
                        //we begin a transaction with the smo server.
                        server.ExecutionManager.ConnectionContext.BeginTransaction();


                        //altering the table: adding new column named 'Comment' that is of type varchar(50) and does not allow NULLs
                        //and setting constraint regarding default value, that should be inserted instead of NULL upon altering the table
                        string sqlKodPridaniSloupce = "ALTER TABLE TestDataTable ADD Comment varchar(50)  NOT NULL CONSTRAINT TheNameOfDefaultValueConstraint DEFAULT('default value') WITH VALUES; \n GO";
                        server.ConnectionContext.ExecuteNonQuery(sqlKodPridaniSloupce);

                        //changing the description
                        //first we need to append and prepand the list of descriptions by single quotation marks, so the SQL server interprets it as a string
                            //Note: Under normal circumstance we would pass it ass a parameter, but I couldn't find online how it's done using SMO
                        listOfDescriptions = listOfDescriptions.Select(x => '\'' + x + '\'').ToList();
                        for (int i = 0; i < listOfDescriptions.Count; i++)
                        {
                            string sqlQueryUpdate = "UPDATE TestDataTable SET Comment = " + listOfDescriptions[i] + " Where (Id = " + listOfIds[i] + ")";
                            server.ConnectionContext.ExecuteNonQuery(sqlQueryUpdate);
                        }

                        //dropping the default value constraint from the table definiton.
                        string sqlQueryDropConstraint =
                                "ALTER TABLE " +
                                    "TestDataTable " +
                                "DROP CONSTRAINT " +
                                    "TheNameOfDefaultValueConstraint;";
                        server.ConnectionContext.ExecuteNonQuery(sqlQueryDropConstraint);


                        //Commiting the transaction
                        server.ExecutionManager.ConnectionContext.CommitTransaction();
                    }
                }
                catch
                {
                    server.ExecutionManager.ConnectionContext.RollBackTransaction();

                }
            }
        }

    }
}
