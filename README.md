NhibernateUpdateSchema
======================

Nhibernate Update Schema commande line tool


Usage
--------------------------------------------------------------------------------
[/h|/?]                            -Help

/w:                                -Specifies the working directory target to use (the current directory is the default)

/c:<config-file-name>              -Specifies NHibernate config file to use

/k:<connectionString-key>          -Specifies connection string key to use

/c:<path-to-hibernate-config>      -Specifies NHibernate config file to use

[/a:<assembly[;assembly2...]>]     -Paths to assemblies that contain embedded
                                    .hbm.xml files
                                  
[/m:<assembly[;assembly2...]>]     -Paths to assemblies that contain domain 
                                    classes

[/d:<path[;path]>]                 -Paths to directories containing .hbm.xml 
                                    files

[/f:<assembly[;assembly2...]>]     -Paths to assemblies that contain fluent
                                    mappings (i.e. ClassMap<KeyAuthApp>)

[/s]                               -Generate script, but don't execute

[/v]                               -Print out script

[/o:<Create>|<Update>|<Delete>]    -Operation to perform

[/g:<dataset[;dataset...]>]        -Paths to files that contain the dataset
                                    to be inserted into the database

[/L]                               -Print copyright and license information

NOTES:

1. The assemblies specified using the /a switch may, but are not require to,
contain the domain classes as well as the embedded .hbm.xml files. This allows
developers to separate their mapping files from the domain objects without 
losing the ability to embed the mapping files in an assembly.  If the 
assemblies DO NOT also contain the domain classes, then the /m option should be
used to specify the assemblies that do contain the domain classes.

2. If the /d switch is used, then an error could result if you do not use the 
/m switch to specify the assemblies containing the domain classes.

3. You may combine the /a, /d, and /m switches if there are multiple methods by
which your project loads mapping classes and domain files.

4. Both the /s and /v option will result in the create/update/delete script 
being output to stdio. The difference is that the /s option instucts nst to
only output the script, and not actually execute it against the database

5. The /g option will populate the database with the indicated dataset files.
Please refer to https://bitbucket.org/guibv/fnst/wiki/Command-Line_tool for
more information about the dataset file format.
