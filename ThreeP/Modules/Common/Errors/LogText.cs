namespace Mac.Modules.Errors;

public record LogText
{
    // COMMON
    // public string Test => Format("Test");
    public static string ObjectIsNull => Format("Object is null");
    public static string OperationCancelled => Format("Operation cancelled");
    
    // EXCEPTIONS
public static string? Exception => Format("Exception occurred: ");
    
    // DB

    //? Ok
    public static string SaveOk => Format("Object Saved");
    public static string DeletedOk => Format("Object Deleted");

    //! Error
    public static string CantFetch => Format("Can't Fetch");
    public static string CantFind => Format("Can't Find");
    public static string UpdateFail => Format("Can't Update");
    public static string DeleteFail => Format("Can't Delete");
    public static string AddFail => Format("Can't Add");
    public static string SaveFail => Format("Can't Save in DB");
    public static string NoChangesToSave => Format("No changes to save...");

    
    // API static 
    public static string ApiError => Format("API Error");
    public static string CantGetFromHttpClient => Format("Can't Get From API");
    public static string CantGetFromApi => Format("Can't Get From API");

    
    //FILE static 
    
    //? Ok
    public static string FileSaveOk => Format("File saved");
    
    
    //! Error
    public static string CantUpload => Format("Can't upload file");
    public static string CantSaveFile => Format("Can't save file");
    public static string CantLoadFile => Format("Can't load file");
    public static string FileNotExist => Format("File not exist");

    
    //IMPORT/EXPORT
    //?Ok
    public static string FileImportedOk => Format("File imported");
    
    //! Fail
    public static string FileImportFail => Format("Can't import file");
    
    
    // SETTING 
    public static string SaveSettings => Format("Settings saved.");
    public static string CantSaveSettings => Format("Can't save settings");

    
    
    static string Format(string text) => $"{text}\n";
}