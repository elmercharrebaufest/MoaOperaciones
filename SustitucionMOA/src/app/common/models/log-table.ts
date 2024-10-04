export interface LogTableDto {
    Id: number;
    Date: string;
    Level: string;
    Logger: string;
    Message: string;
    Exception: string;
}

export interface LogTableCountErrors {
    Logger: string;
    Level: string;
    LoggerLevel: string;
    Count: number;
    Errors: LogTableDto[];
}
