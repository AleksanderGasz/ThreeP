namespace Mac.Modules.Errors;

public record LogText
{
	// COMMON
	// public string Test => Format("Test");
	public static string ObjectIsNull => Format("Objct is null");

	// DB

	public static string CantFetch => Format("Can't Fetch");

    public static string CantFind => Format("Can't Find");
    public static string CantUpdate => Format("Can't Update");
    public static string CantDelete => Format( "Can't Delete");
    public static string CantAdd => Format("Can't Add");
    public static string CantSave=> Format( "Can't Save in DB");
    public static string NoChangesToSave => Format( "No changes to save...");
           
    // API static 
    public static string ApiError => Format( "API Error");
    public static string CantGetFromHttpClient => Format( "Can't Get From API");
    public static string CantGetFromApi => Format( "Can't Get From API");
    
    //FILE static 
    public static string CantUpload => Format("Can't upload file");
    public static string CantSaveFile => Format( "Can't save file");
    public static string CantLoadFile => Format( "Can't load file");
    
    // SETTING 
    public static string SaveSettings => Format( "Settings saved.");
    public static string CantSaveSettings => Format( "Can't save settings");

    static string Format(string text)=> $"{text}\n";
    
}

