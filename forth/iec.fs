require io

code listen ( dv -- )
here 1+
$ffff stx, \ dummy address
lsb lda,x  \ one byte more but faster
$ffb1 jsr, \ listen
here 1+ swap !  \ actual address
$00 ldx,# inx,     \ dummy byte
;code

code second ( sa -- )
here 1+
$ffff stx, 
lsb lda,x
$ff93 jsr, \ second
here 1+ swap !
$00 ldx,# inx, 
;code

code talk ( dv -- )
here 1+
$ffff stx, 
lsb lda,x 
$ffb4 jsr, \ talk
here 1+ swap !
$00 ldx,# inx, 
;code

code tksa ( sa -- )
here 1+
$ffff stx, 
lsb lda,x 
$ff96 jsr, \ tksa
here 1+ swap !
$00 ldx,# inx, 
;code

code unlisten ( -- )
here 1+
$ffff stx,
$ffae jsr,
here 1+ swap ! 
$00 ldx,# 
;code

code untalk ( -- )
here 1+
$ffff stx,
$ffab jsr,
here 1+ swap ! 
$00 ldx,# 
;code

code ciout ( chr -- )
here 1+
$ffff stx, 
lsb lda,x 
$ffa8 jsr, \ c
here 1+ swap ! 
$00 ldx,# inx,
;code

code acptr ( -- chr )
dex, w stx, 0 lda,# msb sta,x
$ffa5 jsr, \ acptr
w ldx, lsb sta,x
;code

: iqt readst ioabort ;

: tfname
over + swap do
i c@ ciout loop ;

: send-cmd ( addr len -- )
0 $90 c!
$ba c@ listen iqt
$ff second iqt
?dup if tfname then
unlisten iqt
$ba c@ talk iqt
$6f tksa iqt
acptr readst begin
0= while emit
acptr readst repeat
emit untalk cr ;

: dos source >in @ /string
dup >in +! \ consume buffer
send-cmd ;

: bsave ( addr addr -- addr )
0 $90 c! parse-name
9 listen iqt $f1 second iqt
tfname
unlisten
9 listen $61 second
over dup 100/ ciout $ff and ciout
over dup -
0 do i + dup c@ ciout loop
1+
unlisten
9 listen $e1 second
unlisten
;

: dir parse-name ?dup if
else drop s" $0"
then 0 $90 c!
$ba c@ listen iqt
$f0 second iqt
over + swap do \ transmit filename
i c@ ciout loop
unlisten $ba c@ talk
$60 tksa acptr acptr 2drop \ drop load address
here begin acptr over c! 1+ \ load HERE loop until EOF 
readst until drop untalk
$ba c@ listen $e0 second unlisten
page here rdir ;

