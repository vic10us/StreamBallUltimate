using System;

public class CommandHandler {
    public bool Queue { get; set; } = true;
    public Action<Arrrgs> Handle;
}
