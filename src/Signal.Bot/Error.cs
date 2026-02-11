namespace Signal.Bot;

public record Error(Exception? Exception, ErrorType ErrorType);