import { ErrorHandler, Injectable } from '@angular/core';
import { NGXLogger } from 'ngx-logger';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {

    constructor(private logger: NGXLogger) { }

    handleError(error: any): void {
        const stackTrace = error.stack ? error.stack : null;

        const componentName = this.getComponentNameFromStack(stackTrace);
        this.logger.error(error.message, componentName);
    }

    private getComponentNameFromStack(stack: string): string {
        const match = stack ? stack.match(/at (.*) \(/) : null;
        return match ? match[1] : stack;
    }

    
}