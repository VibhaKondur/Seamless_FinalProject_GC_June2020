

$("select").mousedown(function (e) {
    e.preventDefault();

    var select = this;
    var scroll = select.scrollTop;

    e.target.selected = !e.target.selected;

    setTimeout(function () { select.scrollTop = scroll; }, 0);

    $(select).focus();
}).mousemove(function (e) { e.preventDefault() });


/*function enableTxtBox1() {
    document.getElementById("country_id").disabled = !document.getElementById("radio1").checked;

    document.getElementById("state_id").disabled = document.getElementById("radio1").checked;
    document.getElementById("city_id").disabled = document.getElementById("radio1").checked
}*/



function enableSelectBox() {

    if (document.getElementById("radio1").checked) {

        document.getElementById("country_id").disabled = false;
        document.getElementById("state_id").disabled = true;
        document.getElementById("city_id").disabled = true;

        document.getElementById("radio1").checked = true;
        document.getElementById("radio2").checked = false;
        document.getElementById("radio3").checked = false;
    }
    else if (document.getElementById("radio2").checked) {

        document.getElementById("country_id").disabled = true;
        document.getElementById("state_id").disabled = false;
        document.getElementById("city_id").disabled = true;

        document.getElementById("radio1").checked = false;
        document.getElementById("radio2").checked = true;
        document.getElementById("radio3").checked = false;
    }
    else if (document.getElementById("radio3").checked) {

        document.getElementById("country_id").disabled = true;
        document.getElementById("state_id").disabled = true;
        document.getElementById("city_id").disabled = false;

        document.getElementById("radio1").checked = false;
        document.getElementById("radio2").checked = false;
        document.getElementById("radio3").checked = true;
    }
}