# Bookstore Administration page

## This application was made as a schoolexercise.

### How to use.

1. Clone the project.
2. Drag the contents(BookstoreAdministration.bak) of the folder DB_Backup to a place where you can find it, for easy access put it in the Backup folder in Program Files > Miscrosoft SQL server > MSSQL16 > MSSQL > Backup.
3. Open SSMS 2022, and restore the Database. Right click Databases -> Restore Database. Check Device, click the dots, click add and look for the folder where you put the .Bak file. Add it, and then click OK, and Ok again.
4. Open the project in Visual Studio.
5. In package manager console write Update-Database.

And there you go, Now you should have free reign over the project.

## NOTE! 
## This database is local, so none of the changes you do will affect anything. Only on your local machine. Thats why I sent the backup file with the project.

### Also note, that this project is not without its problems, and its not very dry. I've basically reused the same method a bunch of times. And I have a bunch of Dialogs that are basically the same.
### I know that I could have reused the dialogs and changed the text/layout inside based on what we want to do. I chose to take the clunky route, or rather I thought of it after the fact.
