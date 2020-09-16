grammar RSqlQuery;

LETTER 	: 'a'..'z'|'A'..'Z';
ANY : . ;
selector	: ~('('| ')'| ';'|','|'='|'<'|'>'|' '|'!'|'\''|'"')+;

eval 	:
	or
	;

or	:
	and (','and)*
	;

and 	:
	constraint (';'constraint)*
	;

constraint:
 	group
 	| comparison
;

group	:
	'(' or ')'
	;

comparison
	:
	selector comparator arguments;

comparator
	:
	comp_fiql
	| comp_alt
	;

comp_fiql
	: ('!'|'=')(LETTER|'-')*'=';

comp_alt
	:
	 ('>' | '<')'='?
	;

reserved
	:
	'(' | ')' | ';' | ',' | '=' |  '<' | '>' | ' '| '!'
	;

single_quote
	:
	 '\''('\\\'' | ~('\''))* '\''
	;

double_quote
	:
	 '"'('\\"' | ~('"'))* '"'
	;

arguments
	:
	'(' value ( ',' value)* ')'
	| value
	;
value
	: ~('{'|'}'| '('| ')'| ';'|','|'='|'<'|'>'|' '|'!'|'\''|'"')+
	| single_quote
	| double_quote;