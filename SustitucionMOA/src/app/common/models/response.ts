export interface ApiResponse <T>{
    data: T;
    error?: string;
    info?: string;
    logout?:boolean;
}