# Test Old .NET Packages with GitHub Dependabot

## Quick Setup (10 minutes)

### Step 1: Create Local Project

```bash
# Create directory
mkdir OldDotNetTest
cd OldDotNetTest

# Create .gitignore
cat > .gitignore << 'EOF'
bin/
obj/
*.user
.vs/
.vscode/
*.swp
*.swo
~*
EOF
```

### Step 2: Add Project Files

Copy these files to your directory:
- `OldDotNetApp.csproj` → `OldDotNetApp.csproj`
- `OldProgram.cs` → `Program.cs`

### Step 3: Verify Old Packages

Check what's outdated:

```bash
# Install dotnet outdated tool
dotnet tool install -g dotnet-outdated-tool

# See all outdated packages
dotnet outdated

# Output shows (example):
# Newtonsoft.Json                  11.0.2          13.0.3    (2 versions gap)
# Microsoft.EntityFrameworkCore    2.2.0           8.0.14    (6 versions gap!)
# MediatR                          5.1.0           12.0.0    (7 versions gap!!)
# Serilog                          2.5.0           3.1.1     (1 major version)
# ... 20+ more packages
```

### Step 4: Initialize Git & Push to GitHub

```bash
# Initialize repo
git init
git add .
git commit -m "initial: old .NET packages from 2019 era

This simulates a real production app that hasn't been updated in 5+ years.
Contains:
- .NET Core 2.2 era code (pre-minimal hosting)
- 25+ packages that are 3-7 major versions behind
- Several packages with known security vulnerabilities
- Code that demonstrates breaking changes across major versions

Ready for Dependabot testing."

# Create repo on GitHub (https://github.com/new)
# Then:
git remote add origin https://github.com/YOUR_USERNAME/OldDotNetTest.git
git branch -M main
git push -u origin main
```

### Step 5: Enable Dependabot

**Option A: Web UI (Easiest)**

```
1. Go to: https://github.com/YOUR_USERNAME/OldDotNetTest
2. Click: Settings → Code security and analysis
3. Click: Enable on all three Dependabot options
   ☑ Dependabot alerts
   ☑ Dependabot security updates
   ☑ Dependabot version updates
4. Done!
```

**Option B: Configuration File**

Create `.github/dependabot.yml`:

```yaml
version: 2
updates:
  - package-ecosystem: "nuget"
    directory: "/"
    schedule:
      interval: "daily"
    open-pull-requests-limit: 10
    
    # Optional: auto-merge safe updates
    auto-merge:
      allow-auto-merge: true
      ignore-tests: false
```

Push it:
```bash
git add .github/dependabot.yml
git commit -m "chore: enable Dependabot for NuGet"
git push
```

---

## What Will Happen (Timeline)

### T+0 min: Dependabot Enables
```
GitHub processes your Dependabot settings
```

### T+5-10 min: Scanning
```
Dependabot scans repository
├─ Found 25+ outdated packages
├─ Found 3-5 security vulnerabilities
└─ Preparing pull requests
```

### T+10-15 min: PRs Appear
```
GitHub starts creating Dependabot PRs:

✓ PR #1: Bump Newtonsoft.Json 11.0.2 → 13.0.3
✓ PR #2: Bump log4net 2.0.8 → 2.0.15 (SECURITY)
✓ PR #3: Bump EntityFrameworkCore 2.2.0 → 8.0.14
✓ PR #4: Bump MediatR 5.1.0 → 12.0.0
✓ PR #5: Bump Serilog 2.5.0 → 3.1.1
... and ~20 more

Pull Requests tab shows all 25+ PRs
```

### T+15-20 min: Tests Run
```
GitHub Actions runs tests on each PR:
├─ Some tests PASS ✓ (compatible upgrades)
├─ Some tests FAIL ❌ (breaking changes)
└─ Each shows reasons clearly
```

---

## What You'll See in GitHub

### Pull Requests Tab

```
https://github.com/YOUR_USERNAME/OldDotNetTest/pulls

Shows (example):

[Dependabot] Bump log4net from 2.0.8 to 2.0.15
├─ Status: ✓ Tests passed
├─ Labels: dependencies, security
├─ Description: "Bumps log4net from 2.0.8 to 2.0.15.
│   Fixes CVE-2019-0604 (Remote Code Execution)"
└─ Ready to merge

[Dependabot] Bump Newtonsoft.Json from 11.0.2 to 13.0.3
├─ Status: ✓ Tests passed
├─ Labels: dependencies
├─ Description: "Bumps Newtonsoft.Json from 11.0.2 to 13.0.3.
│   See the changelog for more details."
└─ Ready to merge

[Dependabot] Bump EntityFrameworkCore from 2.2.0 to 8.0.14
├─ Status: ✗ Tests FAILED
├─ Labels: dependencies
├─ Description: "Bumps Microsoft.EntityFrameworkCore..."
├─ Reason: "Breaking changes in EntityFrameworkCore:
│   - DbSet property configuration changed
│   - Query translation updated
│   - Migration system restructured
│   See release notes for migration guide."
└─ Needs manual fix

[Dependabot] Bump MediatR from 5.1.0 to 12.0.0
├─ Status: ✗ Tests FAILED
├─ Labels: dependencies
├─ Description: "Bumps MediatR from 5.1.0 to 12.0.0..."
├─ Reason: "IRequestHandler interface changed
│   IRequest<> API is different
│   Dependency injection registration changed"
└─ Needs manual fix

... and ~20+ more PRs
```

### Security Tab

```
https://github.com/YOUR_USERNAME/OldDotNetTest/security/dependabot

Dependabot Alerts:
├─ log4net: Remote Code Execution (CRITICAL)
│  └─ Auto-fix PR: Update to 2.0.15
│
├─ Newtonsoft.Json: Multiple CVEs
│  └─ Auto-fix PR: Update to 13.0.3
│
├─ System.Net.Http: Deprecated (HIGH)
│  └─ Auto-fix PR: Update or remove
│
└─ ... security fixes listed
```

---

## Real Impact: What You'll Learn

### 1. Security Reality
```
You'll see:
├─ CVE-2019-0604 in log4net (Remote Code Execution!)
├─ Multiple vulnerabilities in old Newtonsoft.Json
├─ Deprecated TLS in System.Net.Http/Security
└─ All have "Auto-fix PRs" ready to merge

Learning: Why staying updated matters for security
```

### 2. Breaking Changes
```
You'll see why these updates break:

MediatR 5.1.0 → 12.0.0:
├─ IRequestHandler<TRequest, TResponse> changed
├─ IRequest interface different
├─ Middleware pipeline reorganized
└─ Test errors show all breaking changes

EntityFrameworkCore 2.2.0 → 8.0.14:
├─ DbSet properties must be virtual in old versions
├─ Query translation completely rewritten
├─ Migrations system restructured
└─ DbContext configuration API changed
```

### 3. Migration Path
```
Dependabot PRs show:
├─ What changed (diff)
├─ Why tests fail (error messages)
├─ Release notes with migration guide
└─ Links to fix documentation

You can see the exact code changes needed.
```

---

## Expected PR Count & Effort

| Update Type | Count | Effort | Time |
|---|---|---|---|
| **CRITICAL** (security) | 3-5 | Auto-merge | Same day |
| **MAJOR** (breaking) | 8-10 | Manual fix | 3-5 days |
| **MINOR/PATCH** | 10-12 | Auto-merge | Same day |
| **TOTAL** | ~25-30 | MIXED | 1-2 weeks |

### Realistic Timeline
```
Day 1: Enable Dependabot
├─ 25+ PRs created
├─ 10-15 auto-merge (safe updates)
└─ 10-15 need manual review (breaking changes)

Day 2-3: Fix breaking changes
├─ Update MediatR code patterns
├─ Update EntityFrameworkCore queries
├─ Update AutoMapper profiles
└─ Re-run tests

Day 4-5: Final testing & deployment
├─ All packages updated
├─ Tests passing
└─ Ready to deploy

Total: 5 days effort for 25 packages
```

---

## Key Insights You'll Gain

### 1. Version Gaps Matter
```
Small gaps (1-2 versions):
└─ Low risk, few breaking changes

Large gaps (4-7 versions):
└─ High risk, substantial code changes needed

This project has gaps of 6-7 versions on major packages!
```

### 2. Incremental Updates are Better
```
BAD: Wait 5 years, update everything at once
├─ Months of work
├─ Massive test matrix
├─ High failure risk
└─ Hard to debug

GOOD: Update every quarter
├─ Small changes each time
├─ Easy to test
├─ Can rollback if needed
└─ Always on supported versions
```

### 3. Automated Tools Help
```
Dependabot:
├─ Identifies outdated packages (automated)
├─ Creates PRs automatically (automated)
├─ Runs tests automatically (automated)
├─ Provides migration guidance (automation helps)

Without it:
├─ Manual npm outdated check
├─ Manual PR creation
├─ Manual test running
└─ Manual fix searching
```

---

## Bonus: Enable CodeQL Too

While you're testing, enable CodeQL for source code analysis:

```yaml
# .github/workflows/codeql.yml
name: CodeQL

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  analyze:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        language: ['csharp']
    
    steps:
      - uses: actions/checkout@v3
      - uses: github/codeql-action/init@v2
        with:
          languages: ${{ matrix.language }}
      - uses: github/codeql-action/autobuild@v2
      - uses: github/codeql-action/analyze@v2
```

Now you'll see:
- Dependabot PRs (outdated packages)
- CodeQL alerts (code vulnerabilities)
- Both working together for complete security!

---

## Summary

✅ **Outdated packages:** 25+ from 2019 era  
✅ **Expected PRs:** 25-30 Dependabot PRs  
✅ **Security vulns:** 3-5 CRITICAL  
✅ **Breaking changes:** 8-10 major updates  
✅ **Learning value:** EXCELLENT (real-world scenario)  
✅ **Setup time:** 10 minutes  
✅ **Test duration:** 5-15 minutes to see all PRs  

**This is a perfect learning project for understanding DevSecOps!** 🚀

---

## Files You Need

1. **OldDotNetApp.csproj** - Project with old packages
2. **OldProgram.cs** - Code using old APIs (rename to Program.cs)
3. **.github/dependabot.yml** - Config (optional)

Push all three to GitHub, enable Dependabot, and watch the magic! ✨
