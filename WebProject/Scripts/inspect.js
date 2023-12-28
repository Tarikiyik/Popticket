const searchIcon = document.getElementById("searchIcon");
const searchInput = document.getElementById("searchInput");

searchIcon.addEventListener("mouseenter", function () {
  searchInput.classList.add("visible");
});

searchIcon.addEventListener("mouseleave", function () {
  setTimeout(function () {
    if (!searchInput.matches(":focus")) {
      searchInput.classList.remove("visible");
    }
  }, 3000);
});

searchInput.addEventListener("blur", function () {
  searchInput.classList.remove("visible");
});

let loginButton = document.getElementById("login");

document.addEventListener("DOMContentLoaded", function () {
  var loginContainer = document.getElementById("logincontainer");
  if (loginContainer) {
    loginContainer.style.visibility = "hidden";
    loginContainer.style.opacity = "0";
  }
});
var loginContainer = document.getElementById("logincontainer");
loginButton.addEventListener("click", function () {
  loginContainer.style.visibility = "visible";
  loginContainer.style.opacity = "1";
  document.body.classList.add("body-blur");
  login_form.style.display = "block";
  register_form.style.display = "none";
});

let closeButton = document.getElementById("closebutton");
closeButton.addEventListener("click", function () {
  loginContainer.style.visibility = "hidden";
  loginContainer.style.opacity = "0";
  document.body.classList.remove("body-blur");
});
let signup_button = document.getElementById("sign-up-button");
let login_form = document.getElementById("login-form");
let register_form = document.getElementById("register-form");
signup_button.addEventListener("click", function () {
  login_form.style.display = "none";
  register_form.style.display = "block";
});
