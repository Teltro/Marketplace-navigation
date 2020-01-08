using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using MarketplaceNavigation.Models;
using MarketplaceNavigation.Orm;

using v4Fragment = Android.Support.V4.App.Fragment;

namespace MarketplaceNavigation.Fragments
{
    public class AutorisationFragment : v4Fragment
    {
        private const string tag = "AutorisationFragment";

        private IUserRepository userRepository;
        private DbHelper DAO;

        private EditText UserNameEditText;
        private EditText PasswordEditText;
        private EditText PasswordRepeatEditText;
        private CheckBox RegisteredCheckBox;
        private Button RegisterButton;

        public AutorisationFragment(DbHelper DAO)
        {
            userRepository = DAO.CreateRepository<User>(null) as IUserRepository;
            this.DAO = DAO;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((MainActivity)Activity).SupportActionBar.Title = "Autorisation";

            View view = inflater.Inflate(Resource.Layout.autorisation_layout, container, false);

            UserNameEditText = view.FindViewById<EditText>(Resource.Id.autorisationUserNameEditText);
            PasswordEditText = view.FindViewById<EditText>(Resource.Id.autorisationPasswordEditText);
            PasswordRepeatEditText = view.FindViewById<EditText>(Resource.Id.autorisationPasswordRepeatEditText);
            RegisteredCheckBox = view.FindViewById<CheckBox>(Resource.Id.autorisationRegisteredCheckBox);
            RegisterButton = view.FindViewById<Button>(Resource.Id.autorisationRegisterButton);

            RegisteredCheckBox.CheckedChange += (sender, e) => RegisteredCheckChange();
            RegisterButton.Click += (sender, e) => RegisterClick();

            return view;
        }

        private void RegisteredCheckChange()
        {
            PasswordRepeatEditText.Text = "";
            if (RegisteredCheckBox.Checked)
            {
                PasswordRepeatEditText.Visibility = ViewStates.Gone;
                RegisterButton.Text = "Log in";
            }
            else
            {
                PasswordRepeatEditText.Visibility = ViewStates.Visible;
                RegisterButton.Text = "Register";
            }
        }

        private void RegisterClick()
        {
            if (RegisteredCheckBox.Checked)
                LogIn();
            else
                Register();
        }

        private void Register()
        {
            User user = new User();
            user.Name = UserNameEditText.Text;
            user.Password = PasswordEditText.Text;
            if (!Validation(user))
                return;
            user.AccessLevel = AccessLevels.Businessman;

            DAO.User = user;
            userRepository.Insert(user);

            Log.Info(tag, $"user id s {DAO.User.Id}");

            View.ClearFocus();
            this.Activity.OnBackPressed();
        }

        private void LogIn()
        {
            User user = new User();
            user.Name = UserNameEditText.Text;
            user.Password = PasswordEditText.Text;

            User foundUser = userRepository.Find(user);
            if (foundUser == null)
            {
                WrongMessage("Wrong username or password");
                return;
            }

            DAO.User = foundUser;

            Log.Info(tag, $"user id s {foundUser.Id}");

            View.ClearFocus();
            this.Activity.OnBackPressed();
        }

        private bool Validation(User user)
        {
            if (user.Name.Length < 3)
            {
                WrongMessage("Username must have at least 3 symbols");
                return false;
            }
            else if ("0123456789".Contains(user.Name[0]))
            {
                WrongMessage("Username can't begin with number");
                return false;
            }
            else if (user.Name.ToLower() == "admin")
            {
                WrongMessage("You can not register with that name");
                return false;
            }
            else if (userRepository.FindByUserName(user.Name) != null)
            {
                WrongMessage("User with same name already exists");
                return false;
            }
            else if (user.Password.Length < 8)
            {
                WrongMessage("Password must hava at least 8 symbols");
                return false;
            }
            else if (PasswordEditText.Text != PasswordRepeatEditText.Text)
            {
                WrongMessage("Passwords is different");
                return false;
            }
            else
                return true;
        }
        private void WrongMessage(string message)
        {
            Toast.MakeText(this.Context, message, ToastLength.Short).Show();
        }
    }
}