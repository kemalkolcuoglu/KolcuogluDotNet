const hex = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 'A', 'B', 'C', 'D', 'E', 'F'];

const buttonElement = document.getElementById('random-button');
const colorElement = document.getElementById('color');
const colorPaletteElement = document.getElementById('color-palette');

buttonElement.addEventListener('click', function () {
  let hexColor = "#";
  for (let i = 0; i < 6; i++) {
    hexColor += hex[getRandomNumber()];
  }
  console.log(hexColor);
  colorElement.style.color = hexColor;
  colorElement.textContent = hexColor;
  colorPaletteElement.style.backgroundColor = hexColor;
});

function getRandomNumber() {
  return Math.floor(Math.random() * hex.length)
}