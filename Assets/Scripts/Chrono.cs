using System;

public class Chrono {

    DateTime init;
    public int minutes { get; set; }
    public int seconds { get; set; }

    public Chrono() {
        
    }

    public void Start() {
        init = DateTime.Now;
    }

    public string getTime() {

        
        DateTime current = DateTime.Now;
        TimeSpan ts = current.Subtract(init);

        minutes = ts.Minutes;
        seconds = ts.Seconds;

        return ts.Hours!=0 ? "--:--" : string.Format("{0:00}:{1:00}", minutes, seconds);
    }

}
