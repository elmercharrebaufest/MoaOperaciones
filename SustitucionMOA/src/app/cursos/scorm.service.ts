import { Injectable } from "@angular/core";

import { Observable, fromEvent } from 'rxjs';
import { filter, map } from "rxjs/operators";

export function storageChangeObservable(storageArea: Storage, filterFn?: (event: StorageEvent) => boolean): Observable<StorageEvent> {
    return fromEvent<StorageEvent>(window, 'storage').pipe(
        filter((event: StorageEvent) =>
            event.storageArea === storageArea && (!filterFn || filterFn(event))),
        map((event: StorageEvent) => event)
    );
}

export enum ScormMode {
    Scorm = "SCORM",
    Scorm2004 = "SCORM2004"
}

export enum CmiOption {
    Completion = "cmi.completion_status",
    Score2004 = "cmi.core.score.raw",
    Score = "cmi.score.raw",
    SuccessStatus = "cmi.success_status",
    SessionTime = "cmi.core.session_time",
    SuspendData = "cmi.suspend_data",
    LessonStatus = "cmi.core.lesson_status",
}

@Injectable()
export class ScormService {
    private windowSCORM: any
    mode: ScormMode = ScormMode.Scorm;

    savedProgress; any = null;

    setWindow(window: any) {
        this.windowSCORM = window
    }

    restartProgress() {
        this.API.loadFromJSON({
            student_data: this.savedProgress,
        })
    }
    initialize(): void {
        if (this.API) {
            this.Initialize('');
        } else {
            console.error('SCORM API not found');
        }
    }

    private get API() {
        if (this.mode === ScormMode.Scorm)
            return this.windowSCORM.API
        if (this.mode === ScormMode.Scorm2004)
            return this.windowSCORM.API_1484_11
    }
    private get Initialize() {
        if (this.mode === ScormMode.Scorm)
            return this.API.LMSInitialize
        if (this.mode === ScormMode.Scorm2004)
            return this.API.Initialize
    }
    private get Finish() {
        if (this.mode === ScormMode.Scorm)
            return this.API.LMSFinish
        if (this.mode === ScormMode.Scorm2004)
            return this.API.Terminate
    }
    private get GetValue() {
        if (this.mode === ScormMode.Scorm)
            return this.API.LMSGetValue
        if (this.mode === ScormMode.Scorm2004)
            return this.API.GetValue
    }
    private get SetValue() {
        if (this.mode === ScormMode.Scorm)
            return this.API.LMSSetValue
        if (this.mode === ScormMode.Scorm2004)
            return this.API.SetValue
    }
    private get Commit() {
        if (this.mode === ScormMode.Scorm)
            return this.API.LMSCommit
        if (this.mode === ScormMode.Scorm2004)
            return this.API.Commit
    }

    finish(): void {
        if (this.API) {
            this.Finish('');
        } else {
            console.error('SCORM API not found');
        }
    }
    reset(): void {
        if (this.API) {
            this.API.reset();
        } else {
            console.error('SCORM API not found');
        }
    }

    // Example function to get value from SCORM 2004
    getValue(parameter: string): string {
        try {
            if (typeof this.GetValue !== 'undefined') {
                return this.GetValue(parameter);
            } else {
                console.error('SCORM 2004 API not found');

                return null;
            }
        } catch {
            return this.windowSCORM.SCORM2004_CallGetValue(parameter)
        }
    }

    // Example function to set value in SCORM 2004
    setValue(parameter: string, value: any): void {
        try {
            if (typeof this.SetValue !== 'undefined') {
                this.SetValue(parameter, value);
            } else {
                console.error('SCORM 2004 API not found');
            }
        } catch {
            this.windowSCORM.SCORM2004_CallSetValue(parameter, value)
        }


    }

    commit(): void {
        if (typeof this.Commit !== 'undefined') {
            this.Commit();
        } else {
            console.error('SCORM 2004 API not found');
        }
    }

    hookOnSetValue() {
        if (this.API && this.API.on) {
            this.API.on("LMSSetValue", function (CMIElement, value) {
                console.log("Set value ->", CMIElement)
                console.log(value)
            });
            this.API.on("LMSGetValue", function (CMIElement, value) {
                console.log("Get value ->", CMIElement)
                console.log(value)
            });
            this.API.on("LMSCommit", function (CMIElement, value) {
                console.log("Commit value ->", CMIElement)
                console.log(value)
            });
        }
    }

    private get loadFromJSON() {
        return this.API.loadFromJSON;
    }

    savedProgressJSON;

    getResult() {
        this.savedProgressJSON = this.API.cmi.toJSON();
        return this.API.cmi.toJSON()
    }

    showDebug() {
        this.windowSCORM.ShowDebugWindow()
    }

    restartProgressJSON() {
        this.finish()
        this.reset()
        this.loadFromJSON(this.savedProgressJSON)
        this.initialize()
    }

    /**
     * Return the course window
     */
    inicializarCurso(hrefCurso: string): Window {
        const target = '_blank';
        const windowOpen = window.open(hrefCurso, target, 'popup=yes')
        this.setWindow(windowOpen);
        return windowOpen;
    }
}