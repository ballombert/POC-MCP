#!/bin/bash

# Configuration script to set up GitHub URLs for ballombert
# This script updates all repository URLs in the project

set -e

GITHUB_USERNAME="ballombert"
REPO_NAME="POC-MCP"

echo "🔧 Configuring GitHub URLs for ${GITHUB_USERNAME}/${REPO_NAME}"
echo "=================================================="

# Initialize git if not already done
if [ ! -d ".git" ]; then
    echo "📦 Initializing Git repository..."
    git init
    git add .
    git commit -m "Initial commit: .NET MCP Server implementation

- Complete MCP server with .NET 9.0
- Centralized versioning system
- Cross-platform support
- Comprehensive documentation
- CI/CD pipeline ready
- Professional project structure"
fi

# Add GitHub remote
echo "🌐 Setting up GitHub remote..."
if git remote get-url origin >/dev/null 2>&1; then
    echo "   Updating existing remote..."
    git remote set-url origin https://github.com/${GITHUB_USERNAME}/${REPO_NAME}.git
else
    echo "   Adding new remote..."
    git remote add origin https://github.com/${GITHUB_USERNAME}/${REPO_NAME}.git
fi

# Set main branch
echo "🌿 Setting up main branch..."
git branch -M main

echo ""
echo "✅ Configuration complete!"
echo ""
echo "📋 Next Steps:"
echo "=============="
echo ""
echo "1. Create the GitHub repository:"
echo "   - Go to: https://github.com/new"
echo "   - Repository name: ${REPO_NAME}"
echo "   - Description: .NET MCP Server - Model Context Protocol implementation using Microsoft SDK"
echo "   - Set visibility to: 🌍 Public"
echo "   - ❌ Do NOT initialize with README, .gitignore, or license (we have them already)"
echo "   - Click 'Create repository'"
echo ""

echo "2. Push to GitHub:"
echo "   git push -u origin main"
echo ""

echo "3. After pushing, your repository will be available at:"
echo "   🔗 https://github.com/${GITHUB_USERNAME}/${REPO_NAME}"
echo ""

echo "4. Optional setup:"
echo "   - Enable GitHub Actions (should work automatically)"
echo "   - Set up NuGet API key in repository secrets for automatic publishing"
echo "   - Enable GitHub Pages for documentation hosting"
echo ""

echo "🎉 Your .NET MCP server is ready for the world!"
echo ""
echo "📊 Repository includes:"
echo "- Professional .NET 9.0 MCP server implementation"
echo "- Comprehensive documentation with ADRs"
echo "- Automated CI/CD pipeline"
echo "- Version management tools"
echo "- Cross-platform distribution support"