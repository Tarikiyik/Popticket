document.getElementById("inputPhone").addEventListener("input", function (e) {
  var x = e.target.value.replace(/\D/g, "").match(/(\d{0,10})/);
  e.target.value = x[1];
});

document
  .getElementById("inputBirthday")
  .addEventListener("input", function (e) {
    var x = e.target.value
      .replace(/\D/g, "")
      .match(/(\d{0,2})(\d{0,2})(\d{0,4})/);
    e.target.value = x[1] + (x[2] ? "/" + x[2] : "") + (x[3] ? "/" + x[3] : "");
  });

document.querySelector("form").addEventListener("submit", function (e) {
  var phone = document.getElementById("inputPhone").value;
  var birthday = document.getElementById("inputBirthday").value;
  var phonePattern = /^\d{10}$/;
  var birthdayPattern =
    /^(0[1-9]|[12]\d|3[01])\/(0[1-9]|1[0-2])\/(19|20)\d{2}$/;

  // Check phone number format
  if (!phonePattern.test(phone)) {
    alert("Phone number must be 10 digits");
    e.preventDefault(); // Prevent form submission
  }

  // Check birthday format
  if (!birthdayPattern.test(birthday)) {
    alert("Birthday must be in DD/MM/YYYY format");
    e.preventDefault(); // Prevent form submission
  }
});
