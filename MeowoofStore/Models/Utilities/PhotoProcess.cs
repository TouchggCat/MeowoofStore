
using MeowoofStore.ViewModels;

namespace MeowoofStore.Models.Utilities
{
    public class PhotoProcess
    {
        private readonly IWebHostEnvironment _environment;
        public PhotoProcess(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public  void CreatePhoto<T>(object modelObject, string folderPath,string photoPropertyName,string imageStringPropertyName) where T: class
        {
            var photoProperty = modelObject.GetType().GetProperty(photoPropertyName);          //取 viewModel.Photo 屬性資訊
            var photo = (IFormFile)photoProperty.GetValue(modelObject);
            string photoName = Guid.NewGuid().ToString() + ".jpg";
            var imageStringProperty = modelObject.GetType().GetProperty(imageStringPropertyName);
            imageStringProperty.SetValue(modelObject,photoName);                                                   //viewModel.ImageString = photoName;
            using (var stream = new FileStream(_environment.WebRootPath + folderPath + photoName, FileMode.Create))
            {
                photo.CopyTo(stream);
            }
        }

        public  void DeletePhoto(string folderPath, string photoName)
        {
            string filePath = Path.Combine(_environment.WebRootPath + folderPath, photoName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        //原本放Controller的方法
        //private void SavePhoto(ProductViewModel viewModel, string folderPath)
        //{
        //    string photoName = Guid.NewGuid().ToString() + ".jpg";
        //    viewModel.ImageString = photoName;
        //    //抓路徑 IWebHostEnvironment(見下方)
        //    using (var stream = new FileStream(_environment.WebRootPath + folderPath + photoName, FileMode.Create))
        //    {
        //        viewModel.Photo.CopyTo(stream);
        //    }
        //}
        //private void DeletePhoto(string folderPath, string photoName)
        //{
        //    string filePath = Path.Combine(_environment.WebRootPath+ folderPath, photoName);
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        System.IO.File.Delete(filePath);
        //    }
        //}
    }
}

