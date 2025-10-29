#!/bin/bash

# Post-Setup Script for POC-MCP GitHub Repository
# Run this after creating the GitHub repository to update URLs

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Get GitHub username and repository name
get_github_info() {
    echo "🔧 GitHub Repository Configuration"
    echo "=================================="
    echo ""
    
    # Try to get from git remote
    REMOTE_URL=$(git remote get-url origin 2>/dev/null || echo "")
    
    if [[ $REMOTE_URL =~ github\.com[:/]([^/]+)/([^/]+)(\.git)?$ ]]; then
        GITHUB_USERNAME="${BASH_REMATCH[1]}"
        REPO_NAME="${BASH_REMATCH[2]}"
        log_info "Detected from git remote: $GITHUB_USERNAME/$REPO_NAME"
    else
        read -p "Enter your GitHub username: " GITHUB_USERNAME
        read -p "Enter repository name [POC-MCP]: " REPO_NAME
        REPO_NAME=${REPO_NAME:-POC-MCP}
    fi
    
    REPO_URL="https://github.com/$GITHUB_USERNAME/$REPO_NAME"
    
    echo ""
    log_info "Repository: $REPO_URL"
    read -p "Is this correct? (y/N): " CONFIRM
    
    if [[ ! $CONFIRM =~ ^[Yy]$ ]]; then
        log_error "Setup cancelled"
        exit 1
    fi
}

# Update URLs in files
update_urls() {
    log_info "Updating URLs in documentation..."
    
    # Files to update
    FILES=(
        "README.md"
        "CONTRIBUTING.md"
        "Directory.Build.props"
        "docs/getting-started.md"
        "src/SampleMcpServer/.mcp/server.json"
    )
    
    for file in "${FILES[@]}"; do
        if [ -f "$file" ]; then
            # Update YOUR-USERNAME placeholders
            sed -i.bak "s|YOUR-USERNAME|$GITHUB_USERNAME|g" "$file"
            
            # Update your-username (lowercase) placeholders
            sed -i.bak "s|your-username|$GITHUB_USERNAME|g" "$file"
            
            # Update repository name if different
            sed -i.bak "s|POC-MCP|$REPO_NAME|g" "$file"
            
            # Remove backup file
            rm -f "$file.bak"
            
            log_success "Updated $file"
        else
            log_warning "File not found: $file"
        fi
    done
}

# Update .mcp/server.json with proper values
update_mcp_config() {
    log_info "Updating MCP server configuration..."
    
    MCP_FILE="src/SampleMcpServer/.mcp/server.json"
    
    if [ -f "$MCP_FILE" ]; then
        # Create proper MCP configuration
        cat > "$MCP_FILE" << EOF
{
  "\$schema": "https://static.modelcontextprotocol.io/schemas/2025-09-29/server.schema.json",
  "description": ".NET MCP Server - Model Context Protocol implementation with random number generation tools",
  "name": "io.github.$GITHUB_USERNAME.$REPO_NAME",
  "version": "0.1.0-beta",
  "packages": [
    {
      "registryType": "nuget",
      "identifier": "SampleMcpServer",
      "version": "0.1.0-beta",
      "transport": {
        "type": "stdio"
      },
      "packageArguments": [],
      "environmentVariables": []
    }
  ],
  "repository": {
    "url": "$REPO_URL",
    "source": "github"
  }
}
EOF
        log_success "Updated MCP server configuration"
    fi
}

# Show next steps
show_next_steps() {
    echo ""
    echo "🎉 Configuration Complete!"
    echo "========================="
    echo ""
    log_success "All URLs have been updated to point to: $REPO_URL"
    echo ""
    echo "📋 Recommended Next Steps:"
    echo ""
    echo "1. Commit the URL updates:"
    echo "   git add ."
    echo "   git commit -m \"docs: update repository URLs to $REPO_URL\""
    echo "   git push"
    echo ""
    echo "2. Create your first release:"
    echo "   ./scripts/version.sh release  # Remove -beta suffix"
    echo "   git add Directory.Build.props"
    echo "   git commit -m \"release: v0.1.0\""
    echo "   git tag v0.1.0"
    echo "   git push origin main --tags"
    echo ""
    echo "3. Set up NuGet publishing (optional):"
    echo "   - Go to: $REPO_URL/settings/secrets/actions"
    echo "   - Add secret: NUGET_API_KEY"
    echo "   - Value: Your NuGet API key from https://www.nuget.org/account/apikeys"
    echo ""
    echo "4. Enable GitHub Pages for documentation:"
    echo "   - Go to: $REPO_URL/settings/pages"
    echo "   - Source: Deploy from a branch"
    echo "   - Branch: main, Folder: /docs"
    echo ""
    echo "🔗 Your repository URLs:"
    echo "Repository: $REPO_URL"
    echo "Documentation: https://$GITHUB_USERNAME.github.io/$REPO_NAME/docs/"
    echo "Releases: $REPO_URL/releases"
    echo "Issues: $REPO_URL/issues"
    echo ""
    echo "✨ Your .NET MCP server is ready to share with the world!"
}

# Main execution
main() {
    echo "🚀 POC-MCP Post-Setup Configuration"
    echo "==================================="
    echo ""
    
    # Check if we're in a git repository
    if [ ! -d ".git" ]; then
        log_error "Not in a git repository. Please run this from the project root."
        exit 1
    fi
    
    get_github_info
    update_urls
    update_mcp_config
    show_next_steps
}

# Run main function
main "$@"