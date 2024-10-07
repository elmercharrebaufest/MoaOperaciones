import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Chart } from 'chart.js';
import { LogTableCountErrors } from '../common/models/log-table';
import { LogViewerService } from './log-viewer.service';
import { NavService } from '../common/services/NavService';
import { SessionDataService } from '../common/services/SessionDataService';
import { SecurityService } from '../common/services/SecurityService';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ModalService } from '../common/services/ModalService';
import { BaseComponent } from '../common/base-components/base-component';
import { MensajeComponent } from '../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../common/view-child/spinner/spinner.component';

@Component({
  selector: 'app-log-viewer',
  templateUrl: './log-viewer.component.html',
    styleUrls: ['./log-viewer.component.css'],
    providers: [LogViewerService]

})
export class LogViewerComponent extends BaseComponent implements OnInit {
    constructor(protected service: LogViewerService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securytiService, floatMsgService, modalService);
    }
    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    public barChart: any;
    public lineChart: any;

    ngOnInit() {
        this.navService.setSeccionList([]);
        this.createChart();
        
    }
    private createChart() {
        try {
            this.unsubscribe();
            this.subscription = this.service.obtenerLogs().subscribe(
                (result: any) => {
                    //this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        //this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        //this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        //this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        result = result as LogTableCountErrors[];
                        console.log(result);
                        this.createLineChart(result);
                        this.createBarChart(result);
                    }
                },
                error => {
                    //this.spinnerComponent.hideIt();
                    //this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            //this.spinnerComponent.hideIt();
            //this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

    }

    private createBarChart(result: LogTableCountErrors[] ) {   
        const loggers = result.map(data => data.Logger);
        const counts = result.map(data => data.Count);
        this.barChart = new Chart('barChart', {
            type: 'bar',
            data: {
                labels: loggers,
                datasets: [{
                    label: 'Cantidad de Errores',
                    data: counts,
                    backgroundColor: 'rgba(75, 192, 192, 0.2)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                }]
            },
            //options: {
            //    scales: {
            //        y: {
            //            beginAtZero: true
            //        }
            //    }
            //}
        });
                   
        
    }

    private createLineChart(result: LogTableCountErrors[]) {
        const groupedData = this.groupErrorsByHour(result);

        const labels = Object.keys(groupedData);
        const datasets = [];

        const logger = [...new Set(result.map(log => log.Logger))];

        logger.forEach(logger => {
            const data = labels.map(label => groupedData[label][logger] || 0);
            datasets.push({
                label: logger,
                data: data,
                borderColor: this.getRandomColor(),
                fill: false,
                borderWidth: 2,
            });
        });

        // Crear el gráfico
        this.lineChart = new Chart('lineChart', {
            type: 'line',
            data: {
                labels: labels,
                datasets: datasets
            },
            options: {
                responsive: true,
                scales: {
                    x: {
                        title: {
                            display: true,
                            text: 'Minutos'
                        }
                    },
                    y: {
                        title: {
                            display: true,
                            text: 'Cantidad de Errores'
                        }
                    }
                }
            }
        });
    }
    private convertDotNetDate(dateString: string): Date {
        const regex = /\/Date\((\d+)\)\//;
        const match = dateString.match(regex);
        if (match) {
            const timestamp = parseInt(match[1], 10); // Convertir el timestamp a número
            return new Date(timestamp); // Crear un objeto Date
        }
        throw new Error('Invalid date format');
    }
    private groupErrorsByMinute(result: LogTableCountErrors[]) {
        const grouped = {};
        // Asegurarse de que result no sea undefined y tenga elementos
        if (!result || result.length === 0) {
            return grouped; // Retornar objeto vacío si no hay datos
        }

        // Agrupar los datos por minuto
        result.forEach(log => {
            if (log.Errors && Array.isArray(log.Errors)) { // Verificar que errors sea un array
                log.Errors.forEach(error => {
                    const date = this.convertDotNetDate(error.Date); // Convertir la fecha
                    const minute = date.toISOString().substring(0, 16); // Agrupar por minuto (YYYY-MM-DDTHH:MM)
                                    if (!grouped[minute]) {
                        grouped[minute] = {};
                    }
                    if (!grouped[minute][log.Logger]) {
                        grouped[minute][log.Logger] = 0;
                    }
                    grouped[minute][log.Logger] += 1; // Sumar la cantidad de errores
                });
            } else {
                console.warn(`El logger ${log.Logger} no tiene errores válidos.`);
            }
        });
        return grouped;
    }
    private getRandomColor(): string {
        const letters = '0123456789ABCDEF';
        let color = '#';
        for (let i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    }
    private groupErrorsByHour(result: LogTableCountErrors[]) {
        const grouped = {};

        // Asegurarse de que result no sea undefined y tenga elementos
        if (!result || result.length === 0) {
            return grouped; // Retornar objeto vacío si no hay datos
        }

        // Agrupar los datos por hora
        result.forEach(log => {
            if (log.Errors && Array.isArray(log.Errors)) { // Verificar que errors sea un array
                log.Errors.forEach(error => {
                    const date = this.convertDotNetDate(error.Date); // Convertir la fecha
                    const hour = date.toISOString().substring(0, 13); // Agrupar por hora (YYYY-MM-DDTHH)

                    if (!grouped[hour]) {
                        grouped[hour] = {};
                    }
                    if (!grouped[hour][log.Logger]) {
                        grouped[hour][log.Logger] = 0;
                    }
                    grouped[hour][log.Logger] += 1; // Sumar la cantidad de errores
                });
            } else {
                console.warn(`El logger ${log.Logger} no tiene errores válidos.`);
            }
        });

        return grouped;
    }
}