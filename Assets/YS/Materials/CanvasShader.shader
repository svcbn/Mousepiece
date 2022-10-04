Shader "Custom/CanvasShader"
{
    SubShader
    {
        Tags{"Queue" = "Transparent+1"}

        pass
        {
            Blend Zero One
        }
    }
}
