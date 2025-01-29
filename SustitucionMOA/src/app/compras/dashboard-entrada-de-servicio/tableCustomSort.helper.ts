import { SortEvent } from "primeng/api";

export class TableCustomSort {
    public static sortFunction(event: SortEvent, tableName: string, tablesConfig: TableConfig[]): void {
        event.data.sort((data1, data2) => {
            let value1 = data1[event.field];
            let value2 = data2[event.field];
            let result = null;
            let columnType: string | null | undefined =
                tablesConfig
                    .find(x => x.name === tableName)
                    .columns
                    .find(x => x.field === event.field)
                    .type;

            if (value1 == null && value2 != null) {
                result = -1;
            } else if (value1 != null && value2 == null) {
                result = 1;
            } else if (value1 == null && value2 == null) {
                result = 0;
            } else if (columnType === 'date') {
                const value1DateComponents: number[] = value1.split('/');
                const value2DateComponents: number[] = value2.split('/');
                const date1: Date = new Date(value1DateComponents[2], value1DateComponents[1], value1DateComponents[0])
                const date2: Date = new Date(value2DateComponents[2], value2DateComponents[1], value2DateComponents[0])
                result = (date1 < date2) ? -1 : (date1 > date2) ? 1 : 0;
            } else if (typeof value1 === 'string' && typeof value2 === 'string') {
                result = value1.localeCompare(value2);
            } else {
                result = (value1 < value2) ? -1 : (value1 > value2) ? 1 : 0;
            }

            return (event.order * result);
        });
    }
}

interface TableConfig {
    name: string;
    columns: {
        field: string;
        type: string;
    }[];
}