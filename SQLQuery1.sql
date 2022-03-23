Begin Transaction;         
         ALTER TABLE 
                TestDataTable
            ADD 
                Comment varchar(50)  NOT NULL 
            CONSTRAINT  
                TheNameOfDefaultValueConstraint 
            DEFAULT 
                'default value'   
            GO
            UPDATE TestDataTable SET Comment = 'aaaa';
COMMIT;