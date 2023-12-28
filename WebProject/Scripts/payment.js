let cardNumberInput = document.getElementById("cardnumber");
let cardMonthInput = document.getElementById("month");
let cardYearInput = document.getElementById("year");
let cardCvvInput = document.getElementById("cvv");
let cardNameInput = document.getElementById("cardname");

let cardNumberPreview = document.getElementById("card-front-text1");
let cardDatePreview = document.getElementById("card-front-text2");
let cardNamePreview = document.getElementById("card-front-text3");
let cardCvvPreview = document.getElementById("card-back-text");

// Function to update card number preview
cardNumberInput.addEventListener("input", function () {
  cardNumberPreview.textContent = this.value;
});

// Function to update card date preview
cardMonthInput.addEventListener("input", updateDatePreview);
cardYearInput.addEventListener("input", updateDatePreview);

function updateDatePreview() {
  let month = cardMonthInput.value.padStart(2, "0"); // Ensure two digits
  let year = cardYearInput.value.slice(-2); // Take last two digits of year
  cardDatePreview.textContent = `${month}/${year}`;
}

// Function to update card CVV preview
cardCvvInput.addEventListener("input", function () {
  cardCvvPreview.textContent = this.value;
});

// Function to update card name preview
cardNameInput.addEventListener("input", function () {
  cardNamePreview.textContent = this.value;
});
cardNumberInput.addEventListener("input", function (e) {
  // Allow only digits
  let value = this.value.replace(/\D/g, "");
  // Limit length to 16 digits
  this.value = value.slice(0, 16);
});
