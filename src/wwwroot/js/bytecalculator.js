const byteValues = ['Byte', 'KiloByte', 'MegaByte', 'GigaByte', 'TeraByte']

const data = document.getElementById('data');
const from = document.getElementById('from');
const to = document.getElementById('to');
const value = document.getElementById('value');

setInputFilter(data, function(value){
  return /^\d*\.?\d*$/.test(value); // Allow digits and '.' only, using a RegExp
});
data.addEventListener('input', calculate);
from.addEventListener('change', calculate);
to.addEventListener('change', calculate);

function calculate() {
  var difference = from.selectedIndex - to.selectedIndex;
  var result = data.value;
  if (data.value == '' || data.value == '0') {
    text = '0'
    console.log('Nothing Changed!');
  }
  else if (difference == 0) {
    value.textContent = result;
    console.log('Type not Changed!');
  }
  else {
    result = result * Math.pow(1024, difference);
  }
  text = `${data.value} ${byteValues[from.selectedIndex]} = ${result} ${byteValues[to.selectedIndex]}`;
  value.textContent = text;
}

function setInputFilter(textbox, inputFilter) {
  ["input", "keydown", "keyup", "mousedown", "mouseup", "select", "contextmenu", "drop"].forEach(function (event) {
    textbox.addEventListener(event, function () {
      if (inputFilter(this.value)) {
        this.oldValue = this.value;
        this.oldSelectionStart = this.selectionStart;
        this.oldSelectionEnd = this.selectionEnd;
      } else if (this.hasOwnProperty("oldValue")) {
        this.value = this.oldValue;
        this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
      } else {
        this.value = "";
      }
    });
  });
}