d3zk
====

A chart widget library for ZK, based on the d3js library. 

To work on the library: 
<ol>
<li>Clone the git repository</li>
<li>Import the project into Eclipse (File > Import > General > Existing Projects into Workspace)</li>
<li>Add your local ZK libraries to the classpath (Right-click on project > Properties > Java Build Path > Libraries > Add JARS)</li>
</ol>

To use the library in your project: 

<ol>
<li>Clone the git repository</li>
<li>Modify the build.xml file's "lib.dir" property to point to your environment's "zk-web/web/WEB-INF/lib"</li>
<li>run "ant jar" from the command line, copy the resulting idera-d3zk.jar into your webapp libraries (zk-web/web/WEB-INF/lib)</li>
</ol>


Known issues:

When building the project, you may encounter an error parsing the idera-d3zk > src > web.js > d3.min.js file. 
Update to the latest version of Eclipse, that should fix the issue.

Reference: <a href="https://bugs.eclipse.org/bugs/show_bug.cgi?id=356446 target="_blank">Errors running builder 'JavaScript Validator' on project with ArrayIndexOutOfBoundsException</a>