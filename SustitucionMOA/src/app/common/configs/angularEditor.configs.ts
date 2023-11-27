import { AngularEditorConfig } from "@kolkov/angular-editor";

export const GET_ANGULAR_EDITOR_CONFIG: () => AngularEditorConfig = () => ({
    editable: true,
    spellcheck: true,
    height: "auto",
    minHeight: "100px",
    maxHeight: "200px",
    width: "100%",
    minWidth: "100%",
    translate: "yes",
    enableToolbar: true,
    showToolbar: true,
    defaultParagraphSeparator: "",
    defaultFontName: "Arial",
    defaultFontSize: "5",
    fonts: [
        { class: "arial", name: "Arial" },
        { class: "times-new-roman", name: "Times New Roman" },
        { class: "calibri", name: "Calibri" },
        { class: "comic-sans-ms", name: "Comic Sans MS" },
    ],
    customClasses: [
        {
            name: "Quitar enlace",
            class: "quote",
        },
    ],
    uploadUrl: "v1/image",
    sanitize: true,
    toolbarPosition: "top",
})

export const eliminarBotonesExtraEditor = (divToolBar, subscript, superscript, editorTextArea, editorButton) => {

    let toolBars = divToolBar.childNodes;

    if (toolBars.length === 14) {
        let toolBar0 = toolBars[0];
        let toolBar2 = toolBars[2];
        let toolBar3 = toolBars[3];
        let toolBar4 = toolBars[4];
        let toolBar5 = toolBars[5];
        let toolBar6 = toolBars[6];
        let toolBar7 = toolBars[7];
        let toolBar8 = toolBars[8];
        let toolBar9 = toolBars[9];
        let toolBar10 = toolBars[10];
        let toolBar11 = toolBars[11];
        let toolBar13 = toolBars[13];

        divToolBar.removeChild(toolBar0);
        divToolBar.removeChild(toolBar2);
        divToolBar.removeChild(toolBar3);
        divToolBar.removeChild(toolBar4);
        divToolBar.removeChild(toolBar5);
        divToolBar.removeChild(toolBar6);
        divToolBar.removeChild(toolBar7);
        divToolBar.removeChild(toolBar8);
        divToolBar.removeChild(toolBar9);
        divToolBar.removeChild(toolBar10);
        divToolBar.removeChild(toolBar11);
        divToolBar.removeChild(toolBar13);
    }

    subscript.hide();
    superscript.hide();

    editorTextArea.css("font-size", "large");
    editorButton.css("font-size", "large");
}
