import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../common/base-components/base-component';
import { jsPDF } from "jspdf";

@Component({
    selector: 'app-pliego-preview',
    templateUrl: './pliego.preview.component.html',
})
export class PliegoPreviewComponent extends BaseComponent implements OnInit {

    @ViewChild('pdfContent')
    protected pdfContent: ElementRef;
    
    
    ngOnInit() {
        
    }

    testPdf(){
        var doc = new jsPDF();
        
        doc.html(this.pdfContent.nativeElement.innerHTML).then(function(){
            //console.log(doc.output('dataurlstring'));
            doc.save("ejempl.pdf");
        });
    }
}
