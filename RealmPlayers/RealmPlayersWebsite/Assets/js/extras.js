function EnterEvent(buttonName, e) {
    if (e.keyCode == 13) {
        __doPostBack('<%=' + buttonName + '.UniqueID%>', "");
    }
}