using System.Web;

namespace Skybrud.VideoPicker.Models.Videos {

    public interface IVideoEmbedOptions {

        IHtmlString GetHtml();

        IHtmlString GetHtml(int width, int height);

    }

}