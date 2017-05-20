
#include	<windows.h>
#include	<stdio.h>
#include	<stdlib.h>
#include	"xadec.h"


int main(int argc, char *argv[]){
	FILE *fp;
	UCHAR *ps, *pd;
	XAHEADER xah;
	HXASTREAM hxas;
	WAVEFORMATEX wfx;
	XASTREAMHEADER xash;
	ULONG dlen;

	fp = fopen("sample.xa", "rb");

	fread(&xah, 1, sizeof(XAHEADER), fp);
	if ( xah.id != _XAID )
		return(EXIT_FAILURE);

	ps = (UCHAR *)malloc(xah.nDataLen);
	fread(ps, 1, xah.nDataLen, fp);
	fclose(fp);



	hxas = xaDecodeOpen(&xah, &wfx);
	if ( hxas == NULL )
		return(EXIT_FAILURE);

	xaDecodeSize(hxas, xah.nDataLen, &dlen);

	pd = (UCHAR *)malloc(dlen);

	xash.pSrc = ps;
	xash.nSrcLen = xah.nDataLen;
	xash.nSrcUsed = 0;
	xash.pDst = pd;
	xash.nDstLen = dlen;
	xash.nDstUsed = 0;
	xaDecodeConvert(hxas, &xash);

	xaDecodeClose(hxas);

	/* 
	ここまで実行されると、wfxにはWAVEFORMATEXの値、
	pdには展開されたデータが格納されます。
	*/
	fp = fopen("hed.bin", "wb");
	fwrite(&wfx, 1, sizeof(WAVEFORMATEX), fp);
	fclose(fp);

	fp = fopen("out.bin", "wb");
	fwrite(pd, 1, xash.nDstUsed, fp);
	fclose(fp);


	free(pd);
	free(ps);
	return(EXIT_SUCCESS);
}



