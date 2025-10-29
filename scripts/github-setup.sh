#!/bin/bash

# GitHub Setup Script for POC-MCP
# This script helps set up the GitHub repository

echo "🚀 POC-MCP GitHub Setup"
echo "======================="
echo ""

# Check if git is initialized
if [ ! -d ".git" ]; then
    echo "❌ Git not initialized. Run 'git init' first."
    exit 1
fi

# Check if there are commits
if ! git log --oneline -1 >/dev/null 2>&1; then
    echo "❌ No commits found. Make sure you've committed your code."
    exit 1
fi

echo "✅ Git repository is ready for GitHub"
echo ""

echo "📋 Next Steps:"
echo "=============="
echo ""
echo "1. Create GitHub Repository:"
echo "   - Go to: https://github.com/new"
echo "   - Repository name: POC-MCP"
echo "   - Description: .NET MCP Server - Model Context Protocol implementation"
echo "   - Set to: 🌍 Public"
echo "   - ❌ Do NOT initialize with README, .gitignore, or license"
echo "   - Click 'Create repository'"
echo ""

echo "2. After creating the repository, run:"
echo "   Replace YOUR-USERNAME with your actual GitHub username"
echo ""
echo "   git remote add origin https://github.com/YOUR-USERNAME/POC-MCP.git"
echo "   git branch -M main"
echo "   git push -u origin main"
echo ""

echo "3. Optional: Set up repository secrets for CI/CD"
echo "   - Go to Settings > Secrets and variables > Actions"
echo "   - Add secret: NUGET_API_KEY (for automatic NuGet publishing)"
echo ""

echo "4. Enable GitHub Pages (for documentation):"
echo "   - Go to Settings > Pages"
echo "   - Source: Deploy from a branch"
echo "   - Branch: main, folder: /docs"
echo ""

echo "🎉 Your repository will include:"
echo "- Complete .NET MCP server implementation"
echo "- Professional documentation"
echo "- CI/CD pipeline with GitHub Actions"
echo "- Version management tools"
echo "- Issue templates and contributing guidelines"
echo ""

# Show repository stats
echo "📊 Repository Contents:"
echo "======================"
FILES=$(find . -type f -not -path './.git/*' | wc -l | tr -d ' ')
LINES=$(find . -name "*.cs" -o -name "*.md" -o -name "*.json" -o -name "*.yml" -o -name "*.yaml" -o -name "*.sh" | xargs wc -l 2>/dev/null | tail -n1 | awk '{print $1}' || echo "N/A")

echo "📁 Files: $FILES"
echo "📝 Lines of code/docs: $LINES"
echo ""

echo "🔗 Key URLs (after setup):"
echo "=========================="
echo "Repository: https://github.com/YOUR-USERNAME/POC-MCP"
echo "Documentation: https://YOUR-USERNAME.github.io/POC-MCP/docs/"
echo "Releases: https://github.com/YOUR-USERNAME/POC-MCP/releases"
echo "NuGet Package: https://www.nuget.org/packages/SampleMcpServer/"
echo ""

echo "✨ Ready to share your .NET MCP server with the world!"