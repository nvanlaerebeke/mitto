using CommandLineMenu;
using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Messaging.Request;
using Mitto.Messaging.Response;
using System;
using System.Threading.Tasks;

namespace Channel.Client.Actions.Message {

    internal class SpamEcho : BaseAction {

        private string LoremIpsem {
            get {
                //return "";

                return @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse convallis erat nisi, et porta risus congue vitae. Etiam eu sem sit amet augue consectetur lobortis non sed velit. Pellentesque porta, arcu eu dignissim tristique, diam lorem fermentum ligula, vitae consequat erat augue eget mi. Vestibulum nec elit vel metus varius accumsan. Morbi massa eros, sagittis a lobortis sit amet, semper ut nunc. Nam scelerisque, risus sit amet cursus rutrum, mauris ex aliquam odio, mattis interdum mi turpis eget purus. Duis a placerat sapien. Nulla faucibus magna eu nisl molestie facilisis. Mauris tristique metus vitae tellus porttitor finibus. Aliquam viverra eu nibh sed ornare. Cras mattis, dui id commodo interdum, turpis nisl rutrum nibh, eu faucibus nisi sem eget turpis. Nam dapibus a sem ac vehicula.
Phasellus sodales, est at gravida pretium, metus ante sagittis diam, id semper velit magna vitae ligula.Etiam iaculis convallis ex id gravida. Praesent a ligula eu nunc lacinia pellentesque.Nunc eu nunc dictum, elementum eros sed, dictum augue.Aenean iaculis suscipit dignissim. Curabitur faucibus metus at dolor vestibulum, non maximus ipsum pharetra.Sed lobortis suscipit feugiat. Donec at nulla mauris. Pellentesque enim justo, ultricies sed venenatis et, ultricies quis ante.

Nullam commodo dolor turpis, eget tempus elit placerat sed. In interdum nisi id metus cursus, at iaculis arcu interdum.Integer ac blandit lorem, et euismod mi. Donec a purus odio. Nunc eu ante eu ante porttitor tristique.Praesent eget efficitur neque. Morbi egestas tellus non diam malesuada faucibus.In a laoreet massa, et fermentum nibh. Fusce sed sem rutrum leo laoreet fringilla.Nunc molestie sed justo sed iaculis.

Mauris et enim eget massa dictum suscipit eu non massa. Donec ut faucibus orci. Mauris ut metus interdum, tristique nibh sed, egestas dui.Lorem ipsum dolor sit amet, consectetur adipiscing elit.Donec varius orci et massa convallis, sed iaculis magna aliquam.In tempus, dui at dictum sagittis, eros massa hendrerit quam, sed imperdiet velit leo euismod ex.Aliquam erat volutpat.In sagittis turpis quis felis interdum condimentum.Donec pharetra erat ut nibh molestie, ut sagittis tortor tincidunt.Suspendisse potenti. Donec nec semper mauris. Pellentesque eu auctor mauris. Mauris risus mi, tincidunt ut neque sit amet, dapibus volutpat tellus.

Nunc vel erat eget tellus auctor pulvinar ut id magna. Cras suscipit elit eget erat consectetur, ac venenatis erat facilisis.Cras dapibus ultrices purus vel volutpat. Mauris iaculis nisl ac ipsum accumsan consectetur.Morbi cursus erat justo, fermentum consequat metus dictum vel. Morbi aliquet, mi eu posuere consequat, lectus justo tempor ligula, a efficitur dolor turpis nec leo.Suspendisse sit amet ligula sit amet dolor semper porttitor.Nullam dictum magna augue, eu fermentum eros porttitor eget. Sed molestie justo a lectus egestas placerat.Etiam efficitur, quam ac posuere imperdiet, mauris urna porta nisi, non tristique odio tortor sit amet sem. Sed vestibulum consequat molestie. Quisque lacus nisl, rutrum nec nibh id, hendrerit sagittis ex. Phasellus tellus ex, pharetra sit amet est vitae, malesuada facilisis arcu.

Praesent quis aliquet arcu, nec scelerisque leo. Nullam malesuada ligula a nisl suscipit volutpat.Cras odio erat, suscipit eget purus nec, luctus vehicula augue. Curabitur quis scelerisque ligula. Phasellus eget consectetur sem. Maecenas volutpat lorem felis, molestie eleifend sem faucibus eu. Ut cursus placerat sodales. Suspendisse sapien urna, consequat sit amet turpis nec, aliquam facilisis lectus.Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.

Quisque at lectus non libero gravida tempor.Nulla semper eget nunc et viverra. Donec quis nisl eu nisi cursus ultricies in in augue.Praesent iaculis odio sit amet tortor bibendum fermentum at sed tortor.Fusce non fermentum dolor. Pellentesque ut interdum felis. Integer a ipsum rhoncus, iaculis lorem sit amet, tempor sem. Etiam vitae rutrum tortor. Fusce augue magna, suscipit sed est at, consequat feugiat arcu. Nulla tincidunt, risus in dignissim aliquet, nisi dolor pretium dui, ut hendrerit magna eros eu arcu. Nam a sem est.

Etiam eu feugiat nunc, non pharetra magna. Vestibulum vulputate tincidunt nunc, fringilla interdum est eleifend quis. In bibendum eros vestibulum sapien congue, sed ultrices dolor laoreet.Donec eget dignissim neque. Mauris tempor arcu efficitur ipsum eleifend, et ornare dui egestas.Nullam sit amet ante eu odio fermentum efficitur. Pellentesque sollicitudin neque a lacus bibendum elementum.Mauris rhoncus scelerisque elit non aliquet. Curabitur efficitur tempus ipsum sit amet convallis.Ut ultrices urna quis quam tempus ornare.Vivamus vel placerat massa. Duis maximus pharetra nibh at gravida. Etiam vel hendrerit nulla. Fusce pellentesque congue auctor.

Ut vehicula mauris libero, nec consequat metus ornare eu. Ut lacinia, arcu vel fringilla elementum, felis urna commodo sapien, vitae molestie ex erat vitae nunc.Nam consectetur ac leo ut facilisis. Sed neque erat, cursus ut eleifend eget, volutpat eget neque. Cras et diam erat. Sed in feugiat metus. Quisque maximus, nulla non ullamcorper aliquam, libero ex iaculis tortor, vel dapibus dui massa a neque.Donec vel tristique neque.

Vestibulum facilisis aliquam sapien nec iaculis. Maecenas hendrerit faucibus est, ac elementum sem sodales quis. Praesent est orci, venenatis eu pellentesque porta, porttitor a mauris. Duis et ex ut risus iaculis porttitor non et purus. Integer ante ante, tristique ut sagittis eget, rutrum at justo. Donec mattis lorem leo, nec commodo velit convallis quis. Curabitur iaculis diam nec ex bibendum ornare.Suspendisse ullamcorper sodales mauris, sit amet sagittis mauris laoreet ac.Suspendisse suscipit dapibus justo, vitae tempor lectus luctus sed. Pellentesque rutrum vestibulum eleifend. Maecenas dolor lacus, tempor et orci sit amet, accumsan pulvinar turpis.Morbi tincidunt pulvinar magna, id suscipit massa lacinia sed. Pellentesque id rhoncus diam. Donec blandit est nunc, et interdum lectus pulvinar et. Morbi vitae quam diam.";
            }
        }

        public SpamEcho(State pState) : base(pState) {
        }

        public override void Run() {
            Parallel.For(0, int.MaxValue, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, (i) => {
                var request = new EchoRequest($"{LoremIpsem}");
                State.Client.Request<EchoResponse>(request, (r) => {
                    if (r.Status.State == ResponseState.Success) {
                        Console.WriteLine(r.Message);
                    } else {
                        r.Status.ToString();
                    }
                });
            });
        }
    }
}