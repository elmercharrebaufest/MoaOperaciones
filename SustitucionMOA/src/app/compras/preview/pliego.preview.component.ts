import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../../common/base-components/base-component';
import { jsPDF } from "jspdf";
import { Solp } from '../Solp';

@Component({
    selector: 'pliego-preview',
    templateUrl: './pliego.preview.component.html',
    styleUrls: ['../compras.component.css'],
})
export class PliegoPreviewComponent extends BaseComponent implements OnInit {

    @Input('model') 
    model: Solp;

    @ViewChild('pdfContent')
    protected pdfContent: ElementRef;
    
    ngOnInit() {
        
    }

    generarPdf(){
        var doc = new jsPDF();
        
        doc.html(this.pdfContent.nativeElement.innerHTML).then(function(){
            //console.log(doc.output('dataurlstring'));
            doc.save("pliego.pdf");
        });
    }
}
