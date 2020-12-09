let count = 0;

const increaseBtn = document.getElementById('increase');
const decreaseBtn = document.getElementById('decrease');
const resetBtn = document.getElementById('reset');
const value = document.getElementById('value');

resetBtn.addEventListener('click', function () {
  count = 0;
  value.textContent = count;
});

increaseBtn.addEventListener('click', function () {
  count++;
  value.textContent = count;
});

decreaseBtn.addEventListener('click', function () {
  count--;
  value.textContent = count;
});