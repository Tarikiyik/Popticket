using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;
using System.Text.RegularExpressions;

namespace WebProject.Controllers
{
    public class AccountController : Controller
    {
        private PopTicketEntities db = new PopTicketEntities();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Login(string username, string password)
        {
            var user = db.User.FirstOrDefault(u => u.username == username && u.password == password);
            if (user != null)
            {
                Session["User"] = user; // Or use FormsAuthentication.SetAuthCookie
                Session["IsAdmin"] = user.isAdmin;
                return Json(new { success = true, username = user.username });
            }

            return Json(new { success = false, error = "Username or password is wrong" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Register(string name, string surname, string username, string email, string password)
        {
            // Regular expressions for validation
            var regexItem = new Regex("^[a-zA-Z]*$"); // Letters only
            var regexUserPass = new Regex("^[a-zA-Z0-9]{4,16}$"); // Letters and numbers, 8-18 characters

            // Validate input lengths
            if (name.Length < 2 || name.Length > 40 || !regexItem.IsMatch(name) ||
               surname.Length < 2 || surname.Length > 40 || !regexItem.IsMatch(surname))
            {
                return Json(new { success = false, error = "Invalid name or surname" });
            }

            if (!regexUserPass.IsMatch(username))
            {
                return Json(new { success = false, error = "Invalid username" });
            }

            if (password.Length < 8 || password.Length > 18)
            {
                return Json(new { success = false, error = "Invalid password length" });
            }

            // Check for existing user or email
            var existingUser = db.User.Any(u => u.username == username || u.email == email);
            if (existingUser)
            {
                return Json(new { success = false, error = "Username or email already exists" });
            }

            // If all validations pass, proceed to create new user
            var newUser = new User { firstName = name, lastName = surname, username = username, email = email, password = password };
            db.User.Add(newUser);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, error = "An error occurred while creating the account." });
            }

            return Json(new { success = true });
        }

        public ActionResult Logout()
        {
            Session["User"] = null; // Existing code
            Session["IsAdmin"] = null; // Add this line to clear the admin flag
            Session.Clear(); // This will clear all session data
            return RedirectToAction("Homepage", "Home");
        }

        public ActionResult UserProfile(int id)
        {
            var user = Session["User"] as User;
            if (user == null || user.UserID != id)
            {
                return RedirectToAction("Login"); // If not logged in, redirect to login page
            }
            return View(user); // Return the UserProfile view for the logged-in user
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(User updatedUser)
        {
            if (!ModelState.IsValid)
            {
                // Handle validation errors
                return View("UserProfile", updatedUser);
            }

            var user = Session["User"] as User;
            if (user == null)
            {
                // User is not logged in, redirect to login
                return RedirectToAction("Login");
            }

            // Get the user from the current context
            var userInDb = db.User.Find(user.UserID);

            if (userInDb != null)
            {
                // Update the user's properties
                userInDb.phoneNumber = updatedUser.phoneNumber;
                userInDb.birthday = updatedUser.birthday;

                // Save the changes to the database
                db.Entry(userInDb).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                // Update the User object in the session
                Session["User"] = userInDb;

                // Redirect to UserProfile to show the updated information
                return RedirectToAction("UserProfile", new { id = user.UserID });
            }
            else
            {
                // Handle the case where the user could not be found in the database
                ModelState.AddModelError("", "User not found.");
                return View("UserProfile", updatedUser);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccount(int userId)
        {
            var userInDb = db.User.Find(userId);
            if (userInDb == null)
            {
                // Handle the case where the user could not be found in the database
                return HttpNotFound();
            }

            // Remove the user from the database
            db.User.Remove(userInDb);
            db.SaveChanges();

            // Clear the session as the user is now deleted
            Session.Clear();
            return RedirectToAction("Homepage", "Home");

        }
    }
}