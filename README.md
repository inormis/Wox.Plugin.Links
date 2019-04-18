# Links
This plugin let's you to create and manage Links to your most used urls, files, directories.


## Create Link

    link shortcutname path description (optional)
At the moment, plugin doesn't support spaces in the **path** parameter

Examples

|Code|Explanation  | 
|--|--|
| 1. `link gh https://github.com/ \| Open Github` | Creates '**gh**' shortcut what will open 'https://github.com/' url in your default browser, with '*Open Github*' description (visible in the query result)  |
| 2. `link yt https://youtrack.jetbrains.com/issue/RIDER-@@` | Creates '**yt**' shortcut with a required parameter | 
| 3. `link openHello C:\Projects\HelloWorld\HelloWorld.sln` |Opens `HelloWorld.sln` file with your default application| 
| 4. `link projects C:\Projects\` |Opens `C:\Projects\` Directory on your machine | 

## Open shortcut
Type the name of the shortcut. If shortcut requires parameter, pass it separated with a space.
Plugin matches shortcuts if shortcut contains the input (case insencitive)
e.g. Saved shortcuts are

 - OpenAPI 
 - OpenProject 
 - gh 
 - projects

Input:  **ope**, results are: 
 - **Ope**nAPI
 - **Ope**nProject shurtcut

Input: **PRO**, results are: 
- Open**Pro**ject
- **pro**jects

## Export

`-e` or `export`
Open 'Save Dialog' to save the file with links configuration. Can be used to save in a synced folder.

## Import Configuration

    link C:\users\wox\GoogleDrive\links.json
Configures plugin to use given file as a configuration file with links.
## Delete link
`delete shortcut` or `-d shortcut`
Delete given shortcut.
