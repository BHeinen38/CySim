DELETE 
FROM AspNetUsers
WHERE  
Id NOT IN ( SELECT TOP ( 2 )
                           Id
                    FROM    AspNetUsers )