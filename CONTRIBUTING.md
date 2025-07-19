# Contributing

Thanks for considering contributing! 🎉  
This project follows **Conventional Commits** and uses **semantic-release** for fully automated versioning and NuGet publishing.

---

## 🛠 How to contribute

1. **Fork & clone** the repository
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Build & run tests:
   ```bash
   dotnet build
   dotnet test
   ```
4. Create a feature branch:
   ```bash
   git checkout -b feat/add-result-extension
   ```
5. Make your changes and commit using **Conventional Commits** (see below)
6. Push your branch and open a Pull Request 🎉

---

## 📝 Conventional Commits

We use **Conventional Commits** so semantic-release can automatically bump versions and publish to NuGet.

### ✅ Examples

- **feat:** adds a new feature → _minor version bump_

  ```
  feat: add Result.Map extension method
  ```

- **fix:** bug fix → _patch version bump_

  ```
  fix: correct ErrorType serialization issue
  ```

- **feat!:** or add `BREAKING CHANGE:` in the body → _major version bump_

  ```
  feat!: remove legacy CQRS interface

  BREAKING CHANGE: ICommand<T> was removed in favor of ICommandHandler<T>
  ```

### ❌ Avoid

- Messages like `update stuff` or `fix bug`
- Commits without context

---

## 🔄 What happens after merge?

When you merge into **main**:

- semantic-release will analyze commits
- Decide the next version (patch/minor/major)
- Update `CHANGELOG.md` automatically
- Create a GitHub Release
- **Pack & publish a new version to NuGet automatically** 🎉

No manual versioning or tagging is needed!

---

## ✅ Commit Types

| Type     | When to use                                                |
| -------- | ---------------------------------------------------------- |
| feat     | A new feature                                              |
| fix      | A bug fix                                                  |
| docs     | Documentation only changes                                 |
| style    | Code style changes (formatting, missing semi-colons, etc.) |
| refactor | Code change that neither fixes a bug nor adds a feature    |
| test     | Adding or fixing tests                                     |
| chore    | Maintenance tasks (build, tooling, CI, etc.)               |

---

## 📦 Releasing

Releases are **fully automated** via GitHub Actions + semantic-release.  
Just merge your PR, and everything else happens automatically.

---

Thanks for contributing! 🚀
