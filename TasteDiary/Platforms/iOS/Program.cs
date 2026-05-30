using ObjCRuntime;
using UIKit;

namespace TasteDiary;

/// <summary>
/// iOS application entry point. Calls <c>UIApplication.Main</c> to launch the native app host.
/// </summary>
public class Program
{
    /// <summary>Main entry point — starts the iOS application with the registered <see cref="AppDelegate"/>.</summary>
    static void Main(string[] args)
    {
        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
