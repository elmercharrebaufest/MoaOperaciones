export interface ActionResult<T> {
    data: T;
    error: string;
    info: string;
    logout: boolean;
}